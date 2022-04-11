using System.Collections.Generic;
using AncibleCoreCommon;
using AncibleCoreCommon.CommonData.Client;
using AncibleCoreCommon.CommonData.PlayerEvent;
using AncibleCoreCommon.CommonData.Traits;
using AncibleCoreCommon.CommonData.WorldEvent;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.UI.Alerts;
using Assets.Resources.Ancible_Tools.Scripts.UI.FloatingText;
using Assets.Resources.Ancible_Tools.Scripts.UI.Nameplate;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;
using HealWorldEvent = AncibleCoreCommon.CommonData.WorldEvent.HealWorldEvent;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.World_Events
{
    public class UiWorldEventManager : MonoBehaviour
    {
        private static UiWorldEventManager _instance = null;

        [SerializeField] private int _maxWorldEvents = 100;
        [SerializeField] private Color _outgoingDamageColor = Color.white;
        [SerializeField] private Color _incomingDamageColor = Color.white;
        [SerializeField] private Color _otherDamageColor = Color.white;
        [SerializeField] private Color _healColor = Color.white;
        [SerializeField] private Color _deathColor = Color.white;
        
        [SerializeField] private Color _systemEvent = Color.white;
        [SerializeField] private Color _errorColor = Color.white;
        [SerializeField] private Color _playerEvent = Color.white;
        [SerializeField] private Color _experienceEvent = Color.white;
        [SerializeField] private Color _pickupItemEvent = Color.white;
        [SerializeField] private Color _newAbilityEvent = Color.white;
        [SerializeField] private Color _levelUpEvent = Color.white;
        [SerializeField] private Color _goldEvent = Color.white;

        [SerializeField] private RectTransform _content;
        [SerializeField] private VerticalLayoutGroup _grid;
        [SerializeField] private UiWorldEventController _worldEventTemplate;
        [SerializeField] private ScrollRect _scrollRect;
        
        private QueryNetworkObjectDataMessage _queryNetworkObjectDataMsg = new QueryNetworkObjectDataMessage();

        private List<UiWorldEventController> _controllers = new List<UiWorldEventController>();

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            SubscribeToMessages();
        }

        private void ResizeContent()
        {
            while (_instance._controllers.Count > _instance._maxWorldEvents)
            {
                var controller = _instance._controllers[0];
                _instance._controllers.Remove(controller);
                Destroy(controller.gameObject);
            }

            var controllers = _instance._controllers.ToArray();
            var height = _instance._grid.padding.top + (controllers.Length * _instance._grid.spacing);
            for (var i = 0; i < controllers.Length; i++)
            {
                height += controllers[i].RectTransform.rect.height;
            }
            _instance._content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        public static void ShowEvent(string worldEventJson)
        {
            var worldEvent = AncibleUtils.FromJson<WorldEvent>(worldEventJson);
            switch (worldEvent.Type)
            {
                case WorldEventType.Default:
                    break;
                case WorldEventType.Damage:
                    var damageEvent = AncibleUtils.FromJson<DamageEvent>(worldEventJson);
                    if (damageEvent != null)
                    {
                        var text = damageEvent.Text;
                        if (damageEvent.OriginId == ObjectManagerController.PlayerObjectId)
                        {
                            var objName = string.Empty;
                            var obj = damageEvent.TargetId == ObjectManagerController.PlayerObjectId ? ObjectManagerController.PlayerObject : ObjectManagerController.GetWorldObjectById(damageEvent.TargetId);
                            if (obj)
                            {
                                if (obj != ObjectManagerController.PlayerObject)
                                {
                                    _instance._queryNetworkObjectDataMsg.DoAfter = objData => objName = objData.Name;
                                    _instance.SendMessageTo(_instance._queryNetworkObjectDataMsg, obj);
                                }
                                else
                                {
                                    objName = "yourself";
                                }

                                UiFloatingTextManager.ShowFloatingText($"{damageEvent.Amount:n0}", _instance._outgoingDamageColor, obj);
                            }
                            text = StaticMethods.ApplyColorToText($"You deal {damageEvent.Amount} {damageEvent.DamageType} damage to {objName}", _instance._outgoingDamageColor);
                            
                        }
                        else if (damageEvent.TargetId == ObjectManagerController.PlayerObjectId)
                        {
                            var obj = ObjectManagerController.GetWorldObjectById(damageEvent.OriginId);
                            var objName = string.Empty;
                            if (obj)
                            {
                                _instance._queryNetworkObjectDataMsg.DoAfter = objData => objName = objData.Name;
                                _instance.SendMessageTo(_instance._queryNetworkObjectDataMsg, obj);
                            }
                            text = StaticMethods.ApplyColorToText($"You take {damageEvent.Amount} {damageEvent.DamageType} damage from {objName}", _instance._incomingDamageColor);
                            UiFloatingTextManager.ShowFloatingText($"{(damageEvent.Amount > 0 ? $"-{damageEvent.Amount:n0}" : "Resisted")}", _instance._incomingDamageColor, ObjectManagerController.PlayerObject);
                        }
                        else
                        {
                            text = StaticMethods.ApplyColorToText(text, _instance._otherDamageColor);
                        }

                        var controller = Instantiate(_instance._worldEventTemplate, _instance._grid.transform);
                        controller.Setup(text);
                        _instance._controllers.Add(controller);
                    }
                    break;
                case WorldEventType.StatusEffect:
                    var statusEffectEvent = AncibleUtils.FromJson<StatusEffectWorldEvent>(worldEventJson);
                    if (statusEffectEvent != null)
                    {
                        var text = statusEffectEvent.Text;
                        if (statusEffectEvent.OwnerId == ObjectManagerController.PlayerObjectId)
                        {
                            var targetObj = ObjectManagerController.PlayerObjectId == statusEffectEvent.TargetId ? ObjectManagerController.PlayerObject : ObjectManagerController.GetWorldObjectById(statusEffectEvent.TargetId);
                            if (targetObj)
                            {
                                var objName = string.Empty;
                                if (targetObj != ObjectManagerController.PlayerObject)
                                {
                                    var queryNetworkObjDataMsg = MessageFactory.GenerateQueryNetworkObjectDataMsg();
                                    queryNetworkObjDataMsg.DoAfter = data => objName = data.Name;
                                    _instance.SendMessageTo(queryNetworkObjDataMsg, targetObj);
                                    MessageFactory.CacheMessage(queryNetworkObjDataMsg);
                                }
                                else
                                {
                                    objName = "yourself";
                                }
                                if (!string.IsNullOrEmpty(objName))
                                {
                                    text = $"You've {StaticMethods.ApplyColorToText(statusEffectEvent.Effect.ToPastTenseEffectString(), ColorFactoryController.GetColorFromStatusEffect(statusEffectEvent.Effect))} {objName}!";
                                }

                                UiFloatingTextManager.ShowFloatingText(statusEffectEvent.Effect.ToPastTenseEffectString(), ColorFactoryController.GetColorFromStatusEffect(statusEffectEvent.Effect), targetObj);
                            }

                        }
                        else if (statusEffectEvent.TargetId == ObjectManagerController.PlayerObjectId)
                        {
                            var ownerObj = ObjectManagerController.GetWorldObjectById(statusEffectEvent.OwnerId);
                            if (ownerObj)
                            {
                                var objName = string.Empty;
                                var queryNetworkObjDataMsg = MessageFactory.GenerateQueryNetworkObjectDataMsg();
                                queryNetworkObjDataMsg.DoAfter = data => objName = data.Name;
                                _instance.SendMessageTo(queryNetworkObjDataMsg, ownerObj);
                                MessageFactory.CacheMessage(queryNetworkObjDataMsg);
                                if (!string.IsNullOrEmpty(objName))
                                {
                                    text = $"You've been {StaticMethods.ApplyColorToText(statusEffectEvent.Effect.ToPastTenseEffectString(), ColorFactoryController.GetColorFromStatusEffect(statusEffectEvent.Effect))} by {objName}!";
                                }
                            }
                            UiFloatingTextManager.ShowFloatingText(statusEffectEvent.Effect.ToPastTenseEffectString(), ColorFactoryController.GetColorFromStatusEffect(statusEffectEvent.Effect), ObjectManagerController.PlayerObject);
                        }
                        else
                        {
                            var obj = ObjectManagerController.GetWorldObjectById(statusEffectEvent.OwnerId);
                            if (obj)
                            {
                                UiFloatingTextManager.ShowFloatingText(statusEffectEvent.Effect.ToPastTenseEffectString(), ColorFactoryController.GetColorFromStatusEffect(statusEffectEvent.Effect), obj);
                            }
                        }
                        var controller = Instantiate(_instance._worldEventTemplate, _instance._grid.transform);
                        controller.Setup(text);
                        _instance._controllers.Add(controller);
                        
                    }
                    break;
                case WorldEventType.CustomStatus:
                    var customStatusEvent = AncibleUtils.FromJson<CustomStatusWorldEvent>(worldEventJson);
                    if (customStatusEvent != null)
                    {
                        var text = customStatusEvent.Text;
                        var status = customStatusEvent.Status;
                        var floatingStatus = status;
                        var upper = char.ToUpper(status[0]);
                        floatingStatus = floatingStatus.Remove(0, 1).Insert(0, $"{upper}");

                        if (customStatusEvent.OwnerId == ObjectManagerController.PlayerObjectId)
                        {
                            var targetObj = ObjectManagerController.PlayerObjectId == customStatusEvent.TargetId ? ObjectManagerController.PlayerObject : ObjectManagerController.GetWorldObjectById(customStatusEvent.TargetId);
                            if (targetObj)
                            {
                                var objName = string.Empty;
                                if (targetObj != ObjectManagerController.PlayerObject)
                                {
                                    var queryNetworkObjDataMsg = MessageFactory.GenerateQueryNetworkObjectDataMsg();
                                    queryNetworkObjDataMsg.DoAfter = data => objName = data.Name;
                                    _instance.SendMessageTo(queryNetworkObjDataMsg, targetObj);
                                    MessageFactory.CacheMessage(queryNetworkObjDataMsg);
                                }
                                else
                                {
                                    objName = "yourself";
                                }

                                if (!string.IsNullOrEmpty(objName))
                                {
                                    
                                    text = $"{StaticMethods.ApplyColorToText($"You've {status} {objName}!", _instance._outgoingDamageColor)}";
                                }

                                UiFloatingTextManager.ShowFloatingText(floatingStatus, _instance._outgoingDamageColor, targetObj);
                            }

                        }
                        else if (customStatusEvent.TargetId == ObjectManagerController.PlayerObjectId)
                        {
                            var ownerObj = ObjectManagerController.GetWorldObjectById(customStatusEvent.OwnerId);
                            if (ownerObj)
                            {
                                var objName = string.Empty;
                                var objAlignment = CombatAlignment.Neutral;
                                var queryNetworkObjDataMsg = MessageFactory.GenerateQueryNetworkObjectDataMsg();
                                queryNetworkObjDataMsg.DoAfter = data =>
                                {
                                    objName = data.Name;
                                    objAlignment = data.Alignment;
                                };
                                _instance.SendMessageTo(queryNetworkObjDataMsg, ownerObj);
                                MessageFactory.CacheMessage(queryNetworkObjDataMsg);
                                if (!string.IsNullOrEmpty(objName))
                                {
                                    text = $"{StaticMethods.ApplyColorToText($"You've been {status} by {objName}!",objAlignment == CombatAlignment.Player ? _instance._healColor : _instance._incomingDamageColor)}";
                                }
                            }
                            UiFloatingTextManager.ShowFloatingText(floatingStatus, Color.white, ObjectManagerController.PlayerObject);
                        }
                        else
                        {
                            var obj = ObjectManagerController.GetWorldObjectById(customStatusEvent.OwnerId);
                            if (obj)
                            {
                                UiFloatingTextManager.ShowFloatingText(status, Color.white, obj);
                            }
                        }
                        var controller = Instantiate(_instance._worldEventTemplate, _instance._grid.transform);
                        controller.Setup(text);
                        _instance._controllers.Add(controller);
                    }
                    break;
                case WorldEventType.LevelUp:
                    var levelUpEvent = AncibleUtils.FromJson<LevelUpWorldEvent>(worldEventJson);
                    if (levelUpEvent != null)
                    {
                        GameObject obj = null;
                        if (levelUpEvent.OwnerId == ObjectManagerController.PlayerObjectId)
                        {
                            ShowCustomEvent(StaticMethods.ApplyColorToText($"You've reached level {levelUpEvent.Level + 1}!", _instance._levelUpEvent));
                            obj = ObjectManagerController.PlayerObject;
                        }
                        else
                        {
                            ShowCustomEvent(StaticMethods.ApplyColorToText($"{levelUpEvent.OwnerName} has reached level {levelUpEvent.Level + 1}!", _instance._levelUpEvent));
                            obj = ObjectManagerController.GetWorldObjectById(levelUpEvent.OwnerId);
                        }

                        if (obj)
                        {
                            UiFloatingTextManager.ShowFloatingText("LEVEL UP!", _instance._levelUpEvent, obj);
                        }
                    }
                    break;
                case WorldEventType.Heal:
                    var healEvent = AncibleUtils.FromJson<HealWorldEvent>(worldEventJson);
                    if (healEvent != null)
                    {
                        var text = healEvent.Text;
                        if (healEvent.OwnerId == ObjectManagerController.PlayerObjectId)
                        {
                            var objName = string.Empty;
                            var obj = healEvent.TargetId == ObjectManagerController.PlayerObjectId ? ObjectManagerController.PlayerObject : ObjectManagerController.GetWorldObjectById(healEvent.TargetId);
                            if (obj)
                            {
                                if (obj != ObjectManagerController.PlayerObject)
                                {
                                    _instance._queryNetworkObjectDataMsg.DoAfter = objData => objName = objData.Name;
                                    _instance.SendMessageTo(_instance._queryNetworkObjectDataMsg, obj);
                                }
                                else
                                {
                                    objName = "yourself";
                                }

                                UiFloatingTextManager.ShowFloatingText($"{healEvent.Amount:n0}", _instance._healColor, obj);
                            }
                            text = $"You heal {objName} for {StaticMethods.ApplyColorToText($"{healEvent.Amount}", _instance._healColor)})";

                        }
                        else if (healEvent.TargetId == ObjectManagerController.PlayerObjectId)
                        {
                            var obj = ObjectManagerController.GetWorldObjectById(healEvent.OwnerId);
                            var objName = string.Empty;
                            if (obj)
                            {
                                _instance._queryNetworkObjectDataMsg.DoAfter = objData => objName = objData.Name;
                                _instance.SendMessageTo(_instance._queryNetworkObjectDataMsg, obj);
                            }
                            text = $"You are healed {(string.IsNullOrWhiteSpace(objName) ? string.Empty : $"by {objName}")}) for {StaticMethods.ApplyColorToText($"{healEvent.Amount:n0}", _instance._healColor)} ";
                            UiFloatingTextManager.ShowFloatingText($"{healEvent.Amount:n0}", _instance._healColor, ObjectManagerController.PlayerObject);
                        }
                        else
                        {
                            text = StaticMethods.ApplyColorToText(text, Color.white);
                        }

                        var controller = Instantiate(_instance._worldEventTemplate, _instance._grid.transform);
                        controller.Setup(text);
                        _instance._controllers.Add(controller);
                    }
                    break;
                case WorldEventType.Resource:
                    var resourceEvent = AncibleUtils.FromJson<ResourceWorldEvent>(worldEventJson);
                    if (resourceEvent != null)
                    {
                        var text = resourceEvent.Text;
                        if (resourceEvent.OwnerId == ObjectManagerController.PlayerObjectId)
                        {
                            var objName = string.Empty;
                            var obj = resourceEvent.TargetId == ObjectManagerController.PlayerObjectId ? ObjectManagerController.PlayerObject : ObjectManagerController.GetWorldObjectById(resourceEvent.TargetId);
                            if (obj)
                            {
                                if (obj != ObjectManagerController.PlayerObject)
                                {
                                    _instance._queryNetworkObjectDataMsg.DoAfter = objData => objName = objData.Name;
                                    _instance.SendMessageTo(_instance._queryNetworkObjectDataMsg, obj);
                                }
                                else
                                {
                                    objName = "yourself";
                                }

                                UiFloatingTextManager.ShowFloatingText($"{resourceEvent.Amount:n0}", ColorFactoryController.GetColorFromResource(resourceEvent.Resource), obj);
                            }
                            text = $"You give {StaticMethods.ApplyColorToText($"{resourceEvent.Amount} {resourceEvent.Type}", ColorFactoryController.GetColorFromResource(resourceEvent.Resource))} for {objName}";

                        }
                        else if (resourceEvent.TargetId == ObjectManagerController.PlayerObjectId)
                        {
                            var obj = ObjectManagerController.GetWorldObjectById(resourceEvent.OwnerId);
                            var objName = string.Empty;
                            if (obj)
                            {
                                _instance._queryNetworkObjectDataMsg.DoAfter = objData => objName = objData.Name;
                                _instance.SendMessageTo(_instance._queryNetworkObjectDataMsg, obj);
                            }
                            text = $"You gain {StaticMethods.ApplyColorToText($"{resourceEvent.Amount} {resourceEvent.Type}", ColorFactoryController.GetColorFromResource(resourceEvent.Resource))} from {objName}";
                            UiFloatingTextManager.ShowFloatingText($"{resourceEvent.Amount:n0}", ColorFactoryController.GetColorFromResource(resourceEvent.Resource), ObjectManagerController.PlayerObject);
                        }
                        else
                        {
                            text = StaticMethods.ApplyColorToText(text, Color.white);
                        }

                        var controller = Instantiate(_instance._worldEventTemplate, _instance._grid.transform);
                        controller.Setup(text);
                        _instance._controllers.Add(controller);
                    }
                    break;
                case WorldEventType.Cast:
                    var castEvent = AncibleUtils.FromJson<CastWorldEvent>(worldEventJson);
                    if (castEvent != null)
                    {
                        UiNameplateManager.ShowCastEvent(castEvent);
                    }
                    break;
                case WorldEventType.CancelCast:
                    var cancelCastEvent = AncibleUtils.FromJson<CancelCastWorldEvent>(worldEventJson);
                    if (cancelCastEvent != null)
                    {
                        UiNameplateManager.CancelCast(cancelCastEvent);
                    }
                    break;
            }


            _instance.ResizeContent();
        }

        public static void ShowCustomEvent(string text)
        {
            var controller = Instantiate(_instance._worldEventTemplate, _instance._grid.transform);
            controller.Setup(text);
            _instance._controllers.Add(controller);
            _instance.ResizeContent();
        }

        

        public static void ShowCustomErrorEvent(string text)
        {
            var colorText = StaticMethods.ApplyColorToText(text, _instance._errorColor);
            var controller = Instantiate(_instance._worldEventTemplate, _instance._grid.transform);
            controller.Setup(colorText);
            _instance._controllers.Add(controller);
            _instance.ResizeContent();
        }

        void LateUpdate()
        {
            _scrollRect.verticalScrollbar.value = 0f;
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<ClientPlayerEventUpdateMessage>(ClientPlayerEventUpdate);
            gameObject.Subscribe<ClientBuyItemFromShopResultMessage>(ClientBuyItemFromShopResult);
            gameObject.Subscribe<ClientSellItemToShopResultMessage>(ClientSellItemToShopResult);
            gameObject.Subscribe<ClientInteractWithObjectResultMessage>(ClientInteractWithObjectResult);
        }

        private void ClientPlayerEventUpdate(ClientPlayerEventUpdateMessage msg)
        {
            var playerEvents = msg.Events;
            for (var i = 0; i < playerEvents.Length; i++)
            {
                var playerEvent = AncibleUtils.FromJson<PlayerEvent>(playerEvents[i]);
                if (playerEvent != null)
                {
                    switch (playerEvent.EventType)
                    {
                        case PlayerEventType.General:
                            break;
                        case PlayerEventType.UseItem:
                            var useItemEvent = AncibleUtils.FromJson<PlayerUsedItemEvent>(playerEvents[i]);
                            if (useItemEvent != null)
                            {
                                var item = ItemFactoryController.GetItemByName(useItemEvent.Item);
                                if (item)
                                {
                                    var eventText = $"{StaticMethods.ApplyColorToText($"You used {item.DisplayName}", _playerEvent)}";
                                    var controller = Instantiate(_worldEventTemplate, _grid.transform);
                                    controller.Setup(eventText);
                                    _controllers.Add(controller);
                                }
                            }
                            break;
                        case PlayerEventType.EquipItem:
                            break;
                        case PlayerEventType.UnequipItem:
                            break;
                        case PlayerEventType.Warning:
                            ShowCustomErrorEvent(playerEvent.EventMessage);
                            break;
                        case PlayerEventType.Experience:
                            var experieceEvent = AncibleUtils.FromJson<PlayerExperienceEvent>(playerEvents[i]);
                            if (experieceEvent != null)
                            {
                                UiFloatingTextManager.ShowFloatingText($"+{experieceEvent.Amount:n0} XP", _experienceEvent, ObjectManagerController.PlayerObject);
                            }
                            break;
                        case PlayerEventType.PickupItem:
                            var pickupItemEvent = AncibleUtils.FromJson<PickupItemPlayerEvent>(playerEvents[i]);
                            if (pickupItemEvent != null)
                            {
                                var item = ItemFactoryController.GetItemByName(pickupItemEvent.ItemId);
                                if (item)
                                {
                                    ShowCustomEvent(StaticMethods.ApplyColorToText($"You picked up an item: {item.DisplayName}{(pickupItemEvent.Stack > 1 ? $"x{pickupItemEvent.Stack}" : string.Empty)}", _pickupItemEvent));
                                }
                                
                            }

                            break;
                        case PlayerEventType.NewAbility:
                            var newAbilityEvent = AncibleUtils.FromJson<NewAbilityPlayerEvent>(playerEvents[i]);
                            if (newAbilityEvent != null)
                            {
                                var ability = AbilityFactoryController.GetAbilityFromName(newAbilityEvent.Ability);
                                if (ability)
                                {
                                    ShowCustomEvent(StaticMethods.ApplyColorToText($"You've learned a new ability: {ability.DisplayName}", _newAbilityEvent));
                                    UiAlertManager.ShowAlert($"New Ability: {ability.DisplayName}");
                                }
                            }
                            break;
                        case PlayerEventType.Gold:
                            var goldEvent = AncibleUtils.FromJson<PlayerGoldEvent>(playerEvents[i]);
                            if (goldEvent != null)
                            {
                                ShowCustomEvent($"You gain {StaticMethods.ApplyColorToText($"{goldEvent.Amount}", _goldEvent)} gold");
                                UiFloatingTextManager.ShowFloatingText($"+{goldEvent.Amount:n0} Gold", _goldEvent, ObjectManagerController.PlayerObject);
                            }
                            break;
                    }
                }
            }
            ResizeContent();
        }

        private void ClientBuyItemFromShopResult(ClientBuyItemFromShopResultMessage msg)
        {
            UiServerStatusTextController.CloseText();
            if (msg.Success)
            {
                var item = ItemFactoryController.GetItemByName(msg.Item);
                if (item)
                {
                    var message = $"Bought {item.DisplayName}{(msg.Stack > 1 ? $" x{msg.Stack}" : string.Empty)} for {msg.Cost} gold";
                    ShowCustomEvent(StaticMethods.ApplyColorToText(message, _systemEvent));
                }
                
            }
            else
            {
                var message = $"Failed to buy item - {msg.Message}";
                ShowCustomErrorEvent(message);
                UiAlertManager.ShowAlert(message);
            }

        }

        private void ClientSellItemToShopResult(ClientSellItemToShopResultMessage msg)
        {
            UiServerStatusTextController.CloseText();
            if (msg.Success)
            {
                var item = ItemFactoryController.GetItemByName(msg.Item);
                if (item)
                {
                    var message = $"Sold {item.DisplayName}{(msg.Stack > 1 ? $" x{msg.Stack}" : string.Empty)} for {msg.Amount} gold";
                    ShowCustomEvent(StaticMethods.ApplyColorToText(message, _systemEvent));
                }

            }
            else
            {
                var message = $"Failed to buy item - {msg.Message}";
                ShowCustomErrorEvent(message);
                UiAlertManager.ShowAlert(message);
            }
        }

        private void ClientInteractWithObjectResult(ClientInteractWithObjectResultMessage msg)
        {
            if (!msg.Success)
            {
                ShowCustomErrorEvent(msg.Message);
            }
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }

    }
}