using System.Collections.Generic;
using System.Linq;
using AncibleCoreCommon;
using AncibleCoreCommon.CommonData.Ability;
using AncibleCoreCommon.CommonData.Client;
using AncibleCoreCommon.CommonData.Traits;
using Assets.Ancible_Tools.Scripts.Hitbox;
using Assets.Resources.Ancible_Tools.Scripts.Server.Abilities;
using Assets.Resources.Ancible_Tools.Scripts.UI;
using Assets.Resources.Ancible_Tools.Scripts.UI.Alerts;
using Assets.Resources.Ancible_Tools.Scripts.UI.ObjectInfo;
using Assets.Resources.Ancible_Tools.Scripts.UI.Windows;
using CreativeSpore.SuperTilemapEditor;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System.WorldSelect
{
    public class WorldSelectController : MonoBehaviour
    {
        public static Ability PositionAbility { get; private set; }
        public static Color Friendly => _instance._friendlyObjColor;
        public static Color Neutral => _instance._neutralObjColor;
        public static Color Enemy => _instance._enemyObjColor;

        private static WorldSelectController _instance = null;

        [SerializeField] private Color _neutralObjColor = Color.white;
        [SerializeField] private Color _friendlyObjColor = Color.white;
        [SerializeField] private Color _enemyObjColor = Color.white;
        [SerializeField] private Color _objectColor = Color.white;

        [SerializeField] private int _maxTargetDistance = 20;

        [SerializeField] private UnitSelectorController _select;
        [SerializeField] private UnitSelectorController _hover;
        [SerializeField] private UnitSelectorController _area;
        [SerializeField] private UnitSelectorController _tileSelector;

        [SerializeField] private CollisionLayer _unitSelectLayer;

        [SerializeField] private UiObjectInfoWindowController _objectInfoTemplate;

        private List<GameObject> _targetsInRange = new List<GameObject>();
        

        private GameObject _hovered = null;
        private GameObject _selected = null;

        private ContactFilter2D _contactFilter = new ContactFilter2D();

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }
            _contactFilter = new ContactFilter2D { useLayerMask = true, layerMask = _unitSelectLayer.ToMask(), useTriggers = true };
            _instance = this;
            _select.ResetSelector(transform);
            _hover.ResetSelector(transform);
            _area.ResetSelector(transform);
            _tileSelector.ResetSelector(transform);
            SubscribeToMessages();
        }

        public static GameObject GetObjectAtPosition(Vector2 position)
        {
            //var contactFilter = new ContactFilter2D { useLayerMask = true, layerMask = _instance._unitSelectLayer.ToMask(), useTriggers = true };
            var results = new RaycastHit2D[1];
            var hitCast = Physics2D.Raycast(CameraController.Camera.ScreenToWorldPoint(position).ToVector2(), Vector2.zero, _instance._contactFilter, results);

            return hitCast > 0 ? results[0].transform.gameObject : null;
            //if (hovered)
            //{
            //    if (msg.Previous.MouseRight && !msg.Current.MouseRight)
            //    {
            //        var id = string.Empty;
            //        var queryNetworkObjDataMsg = new QueryNetworkObjectDataMessage
            //        {
            //            DoAfter = data =>
            //            {
            //                id = data.ObjectId;
            //            }
            //        };
            //        _instance.SendMessageTo(queryNetworkObjDataMsg, hovered);
            //        if (!string.IsNullOrEmpty(id))
            //        {
            //            ClientController.SendMessageToServer(new ClientUseAbilityRequestMessage { Ability = "Attack Ability", TargetId = id });
            //        }
            //    }
            //}
        }

        public static void SetSelectedObject(GameObject obj)
        {
            if (_instance._selected)
            {
                _instance.SendMessageTo(UnselectObjectMessage.INSTANCE, _instance._selected);
            }

            if (obj && obj.activeSelf)
            {
                _instance._selected = obj;
                _instance._select.Select(obj);
                _instance.SendMessageTo(SelectObjectMessage.INSTANCE, obj);
                var queryNetworkObjDataMsg = MessageFactory.GenerateQueryNetworkObjectDataMsg();
                queryNetworkObjDataMsg.DoAfter = data =>
                {
                    switch (data.Alignment)
                    {
                        case CombatAlignment.Neutral:
                            _instance._select.SetColor(_instance._neutralObjColor);
                            break;
                        case CombatAlignment.Player:
                            _instance._select.SetColor(_instance._friendlyObjColor);
                            break;
                        case CombatAlignment.Monster:
                            _instance._select.SetColor(_instance._enemyObjColor);
                            break;
                        case CombatAlignment.Object:
                            _instance._select.SetColor(_instance._objectColor);
                            break;
                    }

                };
                _instance.SendMessageTo(queryNetworkObjDataMsg, obj);
                MessageFactory.CacheMessage(queryNetworkObjDataMsg);

                var infoWindow = UiWindowManager.OpenWindow(_instance._objectInfoTemplate);
                infoWindow.Setup(obj);
            }
            else
            {
                UiWindowManager.CloseWindow(_instance._objectInfoTemplate);
                _instance._select.ResetSelector(_instance.transform);
            }
        }

        public static void SetHoveredObject(GameObject obj)
        {
            if (_instance._hovered)
            {
                _instance.SendMessageTo(UnhoverObjectMessage.INSTANCE, _instance._hovered);
            }
            if (obj)
            {
                _instance._hovered = obj;
                _instance._hover.Select(obj);
                _instance.SendMessageTo(HoverObjectMessage.INSTANCE, obj);
                var queryNetworkObjDataMsg = MessageFactory.GenerateQueryNetworkObjectDataMsg();
                queryNetworkObjDataMsg.DoAfter = data =>
                {
                    switch (data.Alignment)
                    {
                        case CombatAlignment.Neutral:
                            _instance._hover.SetColor(_instance._neutralObjColor);
                            break;
                        case CombatAlignment.Player:
                            _instance._hover.SetColor(_instance._friendlyObjColor);
                            break;
                        case CombatAlignment.Monster:
                            _instance._hover.SetColor(_instance._enemyObjColor);
                            break;
                        case CombatAlignment.Object:
                            _instance._hover.SetColor(_instance._objectColor);
                            break;
                    }
                };
                _instance.SendMessageTo(queryNetworkObjDataMsg, obj);
                MessageFactory.CacheMessage(queryNetworkObjDataMsg);
            }
            else
            {
                _instance._hover.ResetSelector(_instance.transform);
            }

        }

        public static void UseAbilityOnSelected(Ability ability)
        {
            if (_instance._selected)
            {
                var obj = _instance._selected;
                var objAlignment = CombatAlignment.Neutral;
                var objId = string.Empty;
                var isPlayer = false;
                var isNetObj = false;
                if (obj == ObjectManagerController.PlayerObject)
                {
                    objAlignment = CombatAlignment.Player;
                    isPlayer = true;
                    objId = ObjectManagerController.PlayerObjectId;
                }
                else
                {
                    var queryNetworkObjDatMsg = MessageFactory.GenerateQueryNetworkObjectDataMsg();
                    queryNetworkObjDatMsg.DoAfter = data =>
                    {
                        isNetObj = true;
                        objId = data.ObjectId;
                        objAlignment = data.Alignment;
                    };
                    _instance.SendMessageTo(queryNetworkObjDatMsg, obj);
                    MessageFactory.CacheMessage(queryNetworkObjDatMsg);
                }

                if (isPlayer)
                {
                    if ((ability.Alignment == AbilityAlignment.Friendly || ability.Alignment == AbilityAlignment.All)  && (ability.TargetType == TargetType.Both || ability.TargetType == TargetType.Self))
                    {
                        ClientController.SendMessageToServer(new ClientUseAbilityRequestMessage{Ability = ability.name, TargetId = objId});
                        if (ability.Auto)
                        {
                            AutoAbilityController.RegisterAutoAbility(ability, objId);
                        }
                        else
                        {
                            AutoAbilityController.CancelCurrentAutoAbility();
                        }
                        GlobalCooldownController.TriggerGlobalCooldown();
                    }
                    else
                    {
                        UiAlertManager.ShowAlert("Invalid Target");
                    }
                }
                else if (isNetObj)
                {
                    switch (ability.Alignment)
                    {
                        case AbilityAlignment.Friendly:
                            if (objAlignment == CombatAlignment.Player)
                            {
                                ClientController.SendMessageToServer(new ClientUseAbilityRequestMessage { Ability = ability.name, TargetId = objId });
                                if (ability.Auto)
                                {
                                    AutoAbilityController.RegisterAutoAbility(ability, objId);
                                }
                                else
                                {
                                    AutoAbilityController.CancelCurrentAutoAbility();
                                }
                            }
                            else if (ability.TargetType == TargetType.Self || ability.TargetType == TargetType.Both)
                            {
                                ClientController.SendMessageToServer(new ClientUseAbilityRequestMessage { Ability = ability.name, TargetId = ObjectManagerController.PlayerObjectId });
                                if (ability.Auto)
                                {
                                    AutoAbilityController.RegisterAutoAbility(ability, ObjectManagerController.PlayerObjectId);
                                }
                                else
                                {
                                    AutoAbilityController.CancelCurrentAutoAbility();
                                }
                            }
                            else
                            {
                                UiAlertManager.ShowAlert("Invalid Target");
                            }
                            break;
                        case AbilityAlignment.Enemy:
                            if (objAlignment == CombatAlignment.Monster)
                            {
                                ClientController.SendMessageToServer(new ClientUseAbilityRequestMessage { Ability = ability.name, TargetId = objId });
                                if (ability.Auto)
                                {
                                    AutoAbilityController.RegisterAutoAbility(ability, objId);
                                }
                                else
                                {
                                    AutoAbilityController.CancelCurrentAutoAbility();
                                }
                            }
                            else
                            {
                                UiAlertManager.ShowAlert("Invalid Target");
                            }
                            break;
                        case AbilityAlignment.All:
                            ClientController.SendMessageToServer(new ClientUseAbilityRequestMessage { Ability = ability.name, TargetId = objId });
                            if (ability.Auto)
                            {
                                AutoAbilityController.RegisterAutoAbility(ability, objId);
                            }
                            else
                            {
                                AutoAbilityController.CancelCurrentAutoAbility();
                            }
                            break;
                    }
                }
            }
            else
            {
                if ((ability.Alignment == AbilityAlignment.Friendly || ability.Alignment == AbilityAlignment.All) && (ability.TargetType == TargetType.Both || ability.TargetType == TargetType.Self))
                {
                    ClientController.SendMessageToServer(new ClientUseAbilityRequestMessage { Ability = ability.name, TargetId = ObjectManagerController.PlayerObjectId });
                    if (ability.Auto)
                    {
                        AutoAbilityController.RegisterAutoAbility(ability, ObjectManagerController.PlayerObjectId);
                    }
                    else
                    {
                        AutoAbilityController.CancelCurrentAutoAbility();
                    }
                }
                else
                {
                    UiAlertManager.ShowAlert("Invalid Target");
                }
            }
        }

        public static void SetupPositionAbility(Ability ability)
        {
            PositionAbility = ability;
            SetAbilityArea(ability.Range);
        }

        public static void SetAbilityArea(int area)
        {
            _instance._area.SetArea(area);
            _instance._area.SetPosition(ObjectManagerController.PlayerObject.transform.position.ToVector2());
            _instance._area.gameObject.SetActive(true);
        }

        public static void ClearAbilityArea()
        {
            _instance._area.gameObject.SetActive(false);
            _instance._area.SetArea(0);
            
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<ReturnSelectedSelectorMessage>(ReturnSelectedSelector);
            gameObject.Subscribe<ReturnHoveredSelectorMessage>(ReturnHoveredSelector);
            gameObject.Subscribe<UpdateInputStateMessage>(UpdateInputState);
            gameObject.Subscribe<ClientObjectUpdateMessage>(ClientObjectUpdate);
        }

        private void ReturnSelectedSelector(ReturnSelectedSelectorMessage msg)
        {
            if (_instance._selected)
            {
                _instance._selected = null;
                UiWindowManager.CloseWindow(_instance._objectInfoTemplate);
            }
            
            _select.ResetSelector(transform);

        }

        private void ReturnHoveredSelector(ReturnHoveredSelectorMessage msg)
        {
            if (_instance._hovered)
            {
                _instance._hovered = null;
            }
            _hover.ResetSelector(transform);
        }

        private void UpdateInputState(UpdateInputStateMessage msg)
        {
            if (DataController.WorldState == WorldState.Active && msg.Previous.Tab && !msg.Current.Tab)
            {
                var orderedTargets = _targetsInRange.OrderBy(t => (t.transform.position.ToVector2() - ObjectManagerController.PlayerObject.transform.position.ToVector2()).magnitude).ToList();
                var targetIndex = -1;
                if (_selected)
                {
                    targetIndex = orderedTargets.IndexOf(_selected);
                }

                if (targetIndex >= 0)
                {
                    targetIndex++;
                    if (targetIndex >= _targetsInRange.Count)
                    {
                        targetIndex = 0;
                    }
                }
                else
                {
                    targetIndex = 0;
                }

                SetSelectedObject(orderedTargets[targetIndex]);
                
            }

            if (PositionAbility)
            {
                var currentTilePos = CameraController.Camera.ScreenToWorldPoint(msg.Current.MousePosition);
                var currentTile = WorldController.GetTileFromWorldPos(currentTilePos);
                if (currentTile != null)
                {
                    _instance._tileSelector.SetPosition(currentTile.WorldPosition);
                    _instance._tileSelector.gameObject.SetActive(true);
                }
                else
                {
                    _instance._tileSelector.gameObject.SetActive(false);
                }
                if (msg.Previous.MouseLeft && !msg.Current.MouseLeft)
                {
                    if (currentTile != null)
                    {
                        ClientController.SendMessageToServer(new ClientUseAbilityRequestMessage { Ability = PositionAbility.name, Position = currentTile.Position.ToData()});
                    }
                    else
                    {
                        PositionAbility = null;
                        ClearAbilityArea();
                    }
                }
                else if (msg.Previous.MouseRight && !msg.Current.MouseRight)
                {
                    PositionAbility = null;
                    ClearAbilityArea();
                }
            }

            if (_area.gameObject.activeSelf)
            {
                _instance._area.SetPosition(ObjectManagerController.PlayerObject.transform.position.ToVector2());
            }
        }

        private void ClientObjectUpdate(ClientObjectUpdateMessage msg)
        {
            StartCoroutine(StaticMethods.WaitForFrames(1, () =>
                {
                    var targetIds = msg.Objects.Where(o => (o.Position.ToVector() - DataController.ActiveCharacter.Position.ToVector()).magnitude <=
                                                           _maxTargetDistance && o.Alignment == CombatAlignment.Monster).Select(t => t.ObjectId).ToArray();
                    _targetsInRange = targetIds.Select(ObjectManagerController.GetWorldObjectById).ToList();
                }));
        }
    }
}