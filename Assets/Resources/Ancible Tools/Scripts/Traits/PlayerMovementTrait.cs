using System;
using System.Collections.Generic;
using System.Linq;
using AncibleCoreCommon;
using AncibleCoreCommon.CommonData.Client;
using AncibleCoreCommon.CommonData.Combat;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Ancible_Tools.Scripts.System.Maps;
using DG.Tweening;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.Traits
{
    [CreateAssetMenu(fileName = "Player Movement Trait", menuName = "Ancible Tools/Traits/Player Movement")]
    public class PlayerMovementTrait : Trait
    {
        private Rigidbody2D _rigidBody = null;
        private Vector2Int _direction = Vector2Int.zero;
        private Vector2Int _tile = Vector2Int.zero;

        private Tween _moveTween = null;
        private Sequence _delayBeforeDoAfter = null;

        private List<Vector2Int> _tileList = new List<Vector2Int>();
        private List<MapTile> _path = new List<MapTile>();
        private Action _onCompletedPath = null;

        private UpdateTileMessage _updateTileMsg = new UpdateTileMessage();
        private UpdateNextTileMessage _updateNextTileMsg = new UpdateNextTileMessage();


        private bool _dead = false;
        private bool _transfer = false;

        public override void SetupController(TraitController controller)
        {
            base.SetupController(controller);
            _rigidBody = _controller.transform.parent.gameObject.GetComponent<Rigidbody2D>();
            SubscribeToMessags();
        }

        private void MoveToNextPosition()
        {
            if (_moveTween == null && (_direction != Vector2Int.zero || _path.Count > 0) && !_dead && !_transfer)
            {
                var statusEffects = new ClientStatusEffectData[0];
                if (DataController.ActiveCharacter != null)
                {
                    statusEffects = DataController.ActiveCharacter.StatusEffects.Where(e => e.Type == StatusEffectType.Daze || e.Type == StatusEffectType.Root || e.Type == StatusEffectType.Sleep).ToArray();
                }
                if (statusEffects.Length <= 0)
                {
                    if (_path.Count > 0)
                    {
                        if (_tile == _path[0].Position)
                        {
                            _path.RemoveAt(0);

                            if (_path.Count > 0)
                            {
                                _direction = (_path[0].Position - _tile).Normalize();
                            }
                            else
                            {
                                _direction = Vector2Int.zero;
                                if (_onCompletedPath != null)
                                {
                                    _delayBeforeDoAfter = DOTween.Sequence().AppendInterval(WorldTickController.Latency / 1000f + WorldTickController.Discrepency)
                                        .OnComplete(
                                            () =>
                                            {
                                                _delayBeforeDoAfter = null;
                                                _onCompletedPath.Invoke();
                                                _onCompletedPath = null;
                                            });
                                }
                            }
                        }
                    }
                    var updateDirectionMsg = MessageFactory.GenerateUpdateDirectionMsg();
                    updateDirectionMsg.Direction = _direction;
                    _controller.gameObject.SendMessageTo(updateDirectionMsg, _controller.transform.parent.gameObject);
                    MessageFactory.CacheMessage(updateDirectionMsg);
                    if (_direction != Vector2Int.zero)
                    {
                        var nextTile = _direction + _tile;
                        if (WorldController.DoesTileExistOnCurrentMap(nextTile) && !WorldController.IsTileBlocked(nextTile))
                        {
                            _updateNextTileMsg.Tile = nextTile;
                            _controller.gameObject.SendMessageTo(_updateTileMsg, _controller.transform.parent.gameObject);

                            var clientMoveCommandMsg = new ClientMoveCommandMessage { Direction = _direction.ToData() };
                            ClientController.SendMessageToServer(clientMoveCommandMsg);
                            var nextPos = WorldController.GetWorldPositionFromTile(nextTile.ToData());
                            _moveTween = _rigidBody.DOMove(nextPos, WorldTickController.TickRate / 1000f * 6f)
                                .SetEase(Ease.Linear).OnComplete(
                                    () =>
                                    {
                                        _moveTween = null;
                                        _updateTileMsg.Tile = nextTile;
                                        _controller.gameObject.SendMessageTo(_updateTileMsg, _controller.transform.parent.gameObject);
                                        _rigidBody.position = nextPos;
                                        _tile = nextTile;

                                        var updatePositionMsg = MessageFactory.GenerateUpdatePositionMsg();
                                        updatePositionMsg.Position = _rigidBody.position;
                                        _controller.gameObject.SendMessageTo(updatePositionMsg, _controller.transform.parent.gameObject);
                                        MessageFactory.CacheMessage(updatePositionMsg);
                                        MoveToNextPosition();
                                    });
                        }
                    }
                }
            }

            if (_moveTween != null)
            {
                var updatePositionMsg = MessageFactory.GenerateUpdatePositionMsg();
                updatePositionMsg.Position = _rigidBody.position;
                _controller.gameObject.SendMessageTo(updatePositionMsg, _controller.transform.parent.gameObject);
                MessageFactory.CacheMessage(updatePositionMsg);
            }
        }

        private void SubscribeToMessags()
        {
            _controller.gameObject.Subscribe<UpdateTickMessage>(UpdateTick);
            _controller.gameObject.Subscribe<ClientMovementResponseMessage>(ClientMovementResponse);
            _controller.gameObject.Subscribe<ClientPlayerDeadMessage>(ClientPlayerDead);
            _controller.gameObject.Subscribe<ClientPlayerRespawnMessage>(ClientPlayerRespawn);
            _controller.gameObject.Subscribe<RefreshPlayerDataMessage>(RefreshPlayerData);
            _controller.gameObject.Subscribe<ClientTransferToMapMessage>(ClientTransferMap);
            _controller.gameObject.Subscribe<ClientFinishMapTransferMessage>(ClientFinishMapTransfer);

            _controller.transform.parent.gameObject.SubscribeWithFilter<SetDirectionMessage>(SetDirection, _instanceId);
            _controller.transform.parent.gameObject.SubscribeWithFilter<QueryDirectionMessage>(QueryDirection, _instanceId);
            _controller.transform.parent.gameObject.SubscribeWithFilter<SetPositionMessage>(SetPosition, _instanceId);
            _controller.transform.parent.gameObject.SubscribeWithFilter<QueryTilePositionMessage>(QueryTilePosition, _instanceId);
            _controller.transform.parent.gameObject.SubscribeWithFilter<SetMovementPathMessage>(SetMovementPath, _instanceId);
            
            //_controller.transform.parent.gameObject.SubscribeWithFilter<NetworkObjectPositionUpdateMessage>(NetworkObjectPositionUpdate);
        }

        private void SetDirection(SetDirectionMessage msg)
        {
            if (_delayBeforeDoAfter == null && !_dead && !_transfer)
            {
                _direction = msg.Direction;
                if (_path.Count > 0)
                {
                    _path.Clear();
                    _onCompletedPath = null;
                    if (_delayBeforeDoAfter != null)
                    {
                        if (_delayBeforeDoAfter.IsActive())
                        {
                            _delayBeforeDoAfter.Kill();
                        }
                        _delayBeforeDoAfter = null;

                    }
                }

                if (_direction != Vector2Int.zero)
                {
                    MoveToNextPosition();
                }
            }

        }

        private void QueryDirection(QueryDirectionMessage msg)
        {
            msg.DoAfter.Invoke(_direction);
        }

        private void UpdateTick(UpdateTickMessage msg)
        {
            MoveToNextPosition();
        }

        private void ClientMovementResponse(ClientMovementResponseMessage msg)
        {
            if (!msg.Success)
            {
                Debug.Log("Unsuccesful Movement Response");
                if (_moveTween != null)
                {
                    if (_moveTween.IsActive())
                    {
                        _moveTween.Kill();
                    }

                    _moveTween = null;
                }

                _tile = msg.Position.ToVector();
                var pos = WorldController.GetWorldPositionFromTile(msg.Position);
                _rigidBody.position = pos;

                var updatePositionMsg = MessageFactory.GenerateUpdatePositionMsg();
                updatePositionMsg.Position = _rigidBody.position;
                _controller.gameObject.SendMessageTo(updatePositionMsg, _controller.transform.parent.gameObject);
                MessageFactory.CacheMessage(updatePositionMsg);

                _updateTileMsg.Tile = _tile;
                _controller.gameObject.SendMessageTo(_updateTileMsg, _controller.transform.parent.gameObject);
            }
            else
            {
                var maxDistance = (float) WorldTickController.Latency / WorldTickController.TickRate;
                var networkPos = WorldController.GetWorldPositionFromTile(msg.Position);
                var distance = ((networkPos - _rigidBody.position).magnitude) / 5f;

                if (distance > maxDistance)
                {
                    Debug.Log("Position Update Needed");
                    _tile = msg.Position.ToVector();
                    _rigidBody.position = networkPos;

                    var updatePositionMsg = MessageFactory.GenerateUpdatePositionMsg();
                    updatePositionMsg.Position = _rigidBody.position;
                    _controller.gameObject.SendMessageTo(updatePositionMsg, _controller.transform.parent.gameObject);
                    MessageFactory.CacheMessage(updatePositionMsg);

                    _updateTileMsg.Tile = _tile;
                    _controller.gameObject.SendMessageTo(_updateTileMsg, _controller.transform.parent.gameObject);
                }
                else
                {
                    //Debug.Log($"Succesfull Client Movement - {DateTime.Now}");
                }
            }
        }

        private void SetPosition(SetPositionMessage msg)
        {
            _tile = msg.Tile;
            _rigidBody.position = msg.Position;

            var updatePositionMsg = MessageFactory.GenerateUpdatePositionMsg();
            updatePositionMsg.Position = _rigidBody.position;
            _controller.gameObject.SendMessageTo(updatePositionMsg, _controller.transform.parent.gameObject);
            MessageFactory.CacheMessage(updatePositionMsg);

            _updateTileMsg.Tile = _tile;
            _controller.gameObject.SendMessageTo(_updateTileMsg, _controller.transform.parent.gameObject);

            
        }

        private void NetworkObjectPositionUpdate(NetworkObjectPositionUpdateMessage msg)
        {
            var networkPos = WorldController.GetWorldPositionFromTile(msg.Tile.ToData());
            var distance = (networkPos - _rigidBody.position).magnitude;
            var maxDistance = (float) WorldTickController.Latency / WorldTickController.TickRate;

            var actualDistance = distance / 5f;

            if (actualDistance > maxDistance)
            {
                Debug.Log("Player Correction Needed");
                if (_moveTween != null)
                {
                    if (_moveTween.active)
                    {
                        _moveTween.Kill();
                    }

                    _moveTween = null;
                }
                _rigidBody.position = networkPos;
                _tile = msg.Tile;

                var updatePositionMsg = MessageFactory.GenerateUpdatePositionMsg();
                updatePositionMsg.Position = _rigidBody.position;
                _controller.gameObject.SendMessageTo(updatePositionMsg, _controller.transform.parent.gameObject);
                MessageFactory.CacheMessage(updatePositionMsg);
            }
        }

        private void QueryTilePosition(QueryTilePositionMessage msg)
        {
            msg.DoAfter.Invoke(_tile);
        }

        private void SetMovementPath(SetMovementPathMessage msg)
        {
            _path = msg.Path.ToList();
            _direction = _path.Count > 0 ? (_path[0].Position - _tile).Normalize() : Vector2Int.zero;
            _onCompletedPath = msg.OnCompletedPath;
            if (_delayBeforeDoAfter != null)
            {
                if (_delayBeforeDoAfter.IsActive())
                {
                    _delayBeforeDoAfter.Kill();
                }

                _delayBeforeDoAfter = null;
            }
        }

        private void ClientPlayerDead(ClientPlayerDeadMessage msg)
        {
            _path.Clear();
            _tileList.Clear();
            _direction = Vector2Int.zero;
            if (_moveTween != null)
            {
                if (_moveTween.IsActive())
                {
                    _moveTween.Complete();
                }

                _moveTween = null;
            }

            _dead = true;
        }

        private void ClientPlayerRespawn(ClientPlayerRespawnMessage msg)
        {
            _tile = msg.Tile.ToVector();
            var worldPos = WorldController.GetWorldPositionFromTile(msg.Tile);
            _rigidBody.position = worldPos;
            _path.Clear();
            _direction = Vector2Int.zero;
            if (_moveTween != null)
            {
                if (_moveTween.IsActive())
                {
                    _moveTween.Complete();
                }

                _moveTween = null;
            }

            _updateTileMsg.Tile = _tile;
            _controller.gameObject.SendMessageTo(_updateTileMsg, _controller.transform.parent.gameObject);

            var updatePositionMsg = MessageFactory.GenerateUpdatePositionMsg();
            updatePositionMsg.Position = _rigidBody.position;
            _controller.gameObject.SendMessageTo(updatePositionMsg, _controller.transform.parent.gameObject);
            MessageFactory.CacheMessage(updatePositionMsg);
            _dead = false;
        }

        private void RefreshPlayerData(RefreshPlayerDataMessage msg)
        {
            var statusEffects = DataController.ActiveCharacter.StatusEffects.Where(e => e.Type == StatusEffectType.Daze || e.Type == StatusEffectType.Root || e.Type == StatusEffectType.Sleep).ToArray();
            if (statusEffects.Length > 0)
            {
                if (_moveTween != null)
                {
                    _moveTween.Complete();
                    _moveTween = null;
                }

                if (_delayBeforeDoAfter != null)
                {
                    if (_delayBeforeDoAfter.IsActive())
                    {
                        _delayBeforeDoAfter.Kill();
                    }

                    _delayBeforeDoAfter = null;
                }
            }
        }

        private void ClientTransferMap(ClientTransferToMapMessage msg)
        {
            _tileList.Clear();
            if (_moveTween != null)
            {
                if (_moveTween.IsActive())
                {
                    _moveTween.Kill();
                }

                _moveTween = null;
            }

            _direction = Vector2Int.zero;
            _onCompletedPath = null;
            if (_delayBeforeDoAfter != null)
            {
                if (_delayBeforeDoAfter.IsActive())
                {
                    _delayBeforeDoAfter.Kill();
                }

                _delayBeforeDoAfter = null;
            }

            var pos = WorldController.GetWorldPositionFromTile(msg.Tile);
            _tile = msg.Tile.ToVector();
            _rigidBody.position = pos;

            var updatePositionMsg = MessageFactory.GenerateUpdatePositionMsg();
            updatePositionMsg.Position = _rigidBody.position;
            _controller.gameObject.SendMessageTo(updatePositionMsg, _controller.transform.parent.gameObject);
            MessageFactory.CacheMessage(updatePositionMsg);
            _transfer = true;
        }

        private void ClientFinishMapTransfer(ClientFinishMapTransferMessage msg)
        {
            _transfer = false;
        }

        

        public override void Destroy()
        {
            if (_moveTween != null)
            {
                if (_moveTween.IsActive())
                {
                    _moveTween.Kill();
                }

                _moveTween = null;
            }

            if (_delayBeforeDoAfter != null)
            {
                if (_delayBeforeDoAfter.IsActive())
                {
                    _delayBeforeDoAfter.Kill();
                }

                _delayBeforeDoAfter = null;
            }
            base.Destroy();
        }
    }
}