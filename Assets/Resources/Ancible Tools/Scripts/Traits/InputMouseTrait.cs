using System.Linq;
using AncibleCoreCommon;
using AncibleCoreCommon.CommonData;
using AncibleCoreCommon.CommonData.Traits;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Ancible_Tools.Scripts.System.WorldSelect;
using Assets.Resources.Ancible_Tools.Scripts.UI.Alerts;
using Assets.Resources.Ancible_Tools.Scripts.UI.Windows;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.Traits
{
    [CreateAssetMenu(fileName = "Input Mouse Trait", menuName = "Ancible Tools/Traits/Input/Input Mouse")]
    public class InputMouseTrait : Trait
    {
        private Vector2Int _currentTile = Vector2Int.zero;

        private SetMovementPathMessage _setMovementPathMsg = new SetMovementPathMessage();

        public override void SetupController(TraitController controller)
        {
            base.SetupController(controller);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            _controller.Subscribe<UpdateInputStateMessage>(UpdateInputState);
            _controller.transform.parent.gameObject.SubscribeWithFilter<UpdateTileMessage>(UpdateTile, _instanceId);
            _controller.transform.parent.gameObject.SubscribeWithFilter<UpdateNextTileMessage>(UpdateNextTIle, _instanceId);
        }

        private void UpdateInputState(UpdateInputStateMessage msg)
        {
            if (!UiWindowManager.Hovered && !WorldSelectController.PositionAbility)
            {
                var hoveredObj = WorldSelectController.GetObjectAtPosition(msg.Current.MousePosition);
                WorldSelectController.SetHoveredObject(hoveredObj);
                if (msg.Previous.MouseRight && !msg.Current.MouseRight)
                {
                    if (hoveredObj)
                    {
                        var id = string.Empty;
                        var pos = Vector2Int.zero;
                        var alignment = CombatAlignment.Neutral;
                        var interactions = new InteractionType[0];
                        var queryNetworkObjDataMsg = new QueryNetworkObjectDataMessage
                        {
                            DoAfter = data =>
                            {
                                id = data.ObjectId;
                                pos = data.Position.ToVector();
                                alignment = data.Alignment;
                                interactions = data.Interactions.ToArray();
                            }
                        };
                        _controller.SendMessageTo(queryNetworkObjDataMsg, hoveredObj);
                        if (!string.IsNullOrEmpty(id))
                        {
                            var mapTiles = WorldController.GetMapTilesInSquareAreaOnCurrentMap(_currentTile, AbilityFactoryController.AttackAbility.Range);
                            var objTile = mapTiles.FirstOrDefault(t => t.Position == pos);
                            if (objTile == null)
                            {
                                var surroundingTiles = WorldController.GetMapTilesInSquareAreaOnCurrentMap(pos, AbilityFactoryController.AttackAbility.Range);
                                if (surroundingTiles.Length > 0)
                                {
                                    var orderedTiles = surroundingTiles.OrderBy(t => (t.Position - _currentTile).magnitude).ToArray();
                                    var path = WorldController.GetPathFromCurrentMap(_currentTile, orderedTiles[0].Position);
                                    if (path.Length > 0)
                                    {
                                        _setMovementPathMsg.Path = path;
                                        if (alignment == CombatAlignment.Monster)
                                        {
                                            WorldSelectController.SetSelectedObject(hoveredObj);
                                            _setMovementPathMsg.OnCompletedPath = () =>
                                            {
                                                if (!GlobalCooldownController.Active)
                                                {
                                                    ClientController.SendMessageToServer(new ClientUseAbilityRequestMessage { Ability = AbilityFactoryController.AttackAbility.name, TargetId = id });
                                                }
                                                AutoAbilityController.RegisterAutoAbility(AbilityFactoryController.AttackAbility, id);

                                            };
                                        }
                                        else if (interactions.Length > 0)
                                        {
                                            WorldSelectController.SetSelectedObject(hoveredObj);
                                            _setMovementPathMsg.OnCompletedPath = () =>
                                            {
                                                ClientController.SendMessageToServer(new ClientInteractWithObjectRequestMessage{Interaction = interactions[0], ObjectId = id});
                                            };
                                        }
                                        else
                                        {
                                            _setMovementPathMsg.OnCompletedPath = null;
                                        }
                                        this.SendMessageTo(_setMovementPathMsg, _controller.transform.parent.gameObject);
                                    }
                                }

                            }
                            else
                            {
                                if (alignment == CombatAlignment.Monster)
                                {
                                    WorldSelectController.SetSelectedObject(hoveredObj);
                                    if (!GlobalCooldownController.Active)
                                    {
                                        ClientController.SendMessageToServer(new ClientUseAbilityRequestMessage { Ability = AbilityFactoryController.AttackAbility.name, TargetId = id });
                                    }
                                    AutoAbilityController.RegisterAutoAbility(AbilityFactoryController.AttackAbility, id);
                                }
                                else if (interactions.Length > 0)
                                {
                                    WorldSelectController.SetSelectedObject(hoveredObj);
                                    ClientController.SendMessageToServer(new ClientInteractWithObjectRequestMessage { Interaction = interactions[0], ObjectId = id });
                                }
                            }

                        }
                    }
                    else
                    {
                        var worldPosition = CameraController.Camera.ScreenToWorldPoint(msg.Current.MousePosition).ToVector2();
                        var tile = WorldController.GetWorldTileFromPosition(worldPosition);

                        if (tile != _currentTile && WorldController.DoesTileExistOnCurrentMap(tile))
                        {
                            var path = WorldController.GetPathFromCurrentMap(_currentTile, tile);
                            if (path.Length > 0)
                            {
                                _setMovementPathMsg.Path = path;
                                _setMovementPathMsg.OnCompletedPath = null;
                                _controller.gameObject.SendMessageTo(_setMovementPathMsg, _controller.transform.parent.gameObject);
                            }
                        }

                    }
                }
                else if (msg.Previous.MouseLeft && !msg.Current.MouseLeft && !WorldSelectController.PositionAbility)
                {
                    WorldSelectController.SetSelectedObject(hoveredObj);
                }
            }
        }

        private void UpdateTile(UpdateTileMessage msg)
        {
            _currentTile = msg.Tile;
        }

        private void UpdateNextTIle(UpdateNextTileMessage msg)
        {
            _currentTile = msg.Tile;
        }
    }
}