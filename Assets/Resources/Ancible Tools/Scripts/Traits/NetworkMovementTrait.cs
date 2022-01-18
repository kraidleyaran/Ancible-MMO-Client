using System.Collections.Generic;
using Assets.Ancible_Tools.Scripts.System;
using DG.Tweening;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.Traits
{
    [CreateAssetMenu(fileName = "Network Movement Trait", menuName = "Ancible Tools/Traits/Network/Network Movement")]
    public class NetworkMovementTrait : Trait
    {
        private Rigidbody2D _rigidBody;
        private Tween _moveTween = null;
        private Vector2Int _currentTile = Vector2Int.zero;
        private List<Vector2Int> _tileList = new List<Vector2Int>();

        public override void SetupController(TraitController controller)
        {
            base.SetupController(controller);
            _rigidBody = _controller.transform.parent.gameObject.GetComponent<Rigidbody2D>();
            SubscribeToMessages();
        }

        private void MoveToNextPosition()
        {
            if (_tileList.Count > 0 && _moveTween == null)
            {
                var timeToMove = WorldTickController.TickRate / 1000f * 6f;
                var nextTile = _tileList[0];
                _currentTile = nextTile;
                
                var pos = WorldController.GetWorldPositionFromTile(nextTile.ToData());
                var vectorDirection = (pos - _rigidBody.position).normalized;
                var direction = Vector2Int.down;
                if (vectorDirection.x > 0)
                {
                    direction.x = 1;
                    direction.y = 0;
                }
                else if (vectorDirection.x < 0)
                {
                    direction.x = -1;
                    direction.y = 0;
                }
                else if (vectorDirection.y > 0)
                {
                    direction.y = 1;
                }
                else if (vectorDirection.y < 0)
                {
                    direction.y = -1;
                }
                var updateDirectionMsg = MessageFactory.GenerateUpdateDirectionMsg();
                updateDirectionMsg.Direction = direction;
                _controller.gameObject.SendMessageTo(updateDirectionMsg, _controller.transform.parent.gameObject);
                MessageFactory.CacheMessage(updateDirectionMsg);

                _moveTween = _rigidBody.DOMove(pos, timeToMove).SetEase(Ease.Linear).OnComplete(() =>
                {
                    _moveTween = null;
                    _rigidBody.position = pos;
                    _tileList.Remove(nextTile);
                    var updatePositionMsg = MessageFactory.GenerateUpdatePositionMsg();
                    updatePositionMsg.Position = _rigidBody.position;
                    _controller.gameObject.SendMessageTo(updatePositionMsg, _controller.transform.parent.gameObject);
                    MessageFactory.CacheMessage(updatePositionMsg);
                    if (_tileList.Count > 0)
                    {
                        MoveToNextPosition();
                    }
                    else
                    {
                        //var setUnitIdleAnimationStateMsg = MessageFactory.GenerateSetUnitAnimationStateMsg();
                        //setUnitIdleAnimationStateMsg.State = UnitAnimationState.Idle;
                        //_controller.gameObject.SendMessageTo(setUnitIdleAnimationStateMsg, _controller.transform.parent.gameObject);
                        //MessageFactory.CacheMessage(setUnitIdleAnimationStateMsg);
                    }
                });
            }
        }

        private void SubscribeToMessages()
        {
            _controller.gameObject.Subscribe<UpdateTickMessage>(UpdateTick);

            _controller.transform.parent.gameObject.SubscribeWithFilter<NetworkObjectPositionUpdateMessage>(NetworkObjectPositionUpdate, _instanceId);
            _controller.transform.parent.gameObject.SubscribeWithFilter<DisableObjectMessage>(DisableObject, _instanceId);
            _controller.transform.parent.gameObject.SubscribeWithFilter<SetPositionMessage>(SetPosition, _instanceId);
        }

        private void NetworkObjectPositionUpdate(NetworkObjectPositionUpdateMessage msg)
        {
            if (msg.Force || !_controller.transform.parent.gameObject.activeSelf)
            {
                if (_moveTween != null)
                {
                    if (_moveTween.IsActive())
                    {
                        _moveTween.Kill();
                    }

                    _moveTween = null;
                }

                var worldPos = WorldController.GetWorldPositionFromTile(msg.Tile.ToData());
                _currentTile = msg.Tile;
                _rigidBody.position = worldPos;
                _tileList.Clear();
            }
            else
            {
                if (_tileList.Count > 0)
                {
                    if (_tileList[_tileList.Count - 1] != msg.Tile)
                    {
                        _tileList.Add(msg.Tile);
                    }
                }
                else if (msg.Tile != _currentTile)
                {
                    _tileList.Add(msg.Tile);
                }
            }
        }

        private void UpdateTick(UpdateTickMessage msg)
        {
            if (_tileList.Count > 0 && _moveTween == null)
            {
                MoveToNextPosition();
            }

            if (_moveTween != null)
            {
                var updatePositionMsg = MessageFactory.GenerateUpdatePositionMsg();
                updatePositionMsg.Position = _rigidBody.position;
                _controller.gameObject.SendMessageTo(updatePositionMsg, _controller.transform.parent.gameObject);
                MessageFactory.CacheMessage(updatePositionMsg);
            }
        }

        private void DisableObject(DisableObjectMessage msg)
        {
            if (_tileList.Count > 0)
            {
                var lastTile = _tileList[_tileList.Count - 1];
                var pos = WorldController.GetWorldPositionFromTile(lastTile.ToData());
                _rigidBody.position = pos;
                _currentTile = lastTile;
                _tileList.Clear();
            }

            if (_moveTween != null)
            {
                if (_moveTween.IsActive())
                {
                    _moveTween.Kill();
                }

                _moveTween = null;
            }
        }

        private void SetPosition(SetPositionMessage msg)
        {
            _tileList.Clear();
            _currentTile = msg.Tile;
            _rigidBody.position = msg.Position;
            var updatePositionMsg = MessageFactory.GenerateUpdatePositionMsg();
            updatePositionMsg.Position = _rigidBody.position;
            _controller.gameObject.SendMessageTo(updatePositionMsg, _controller.transform.parent.gameObject);
            MessageFactory.CacheMessage(updatePositionMsg);
        }
    }
}