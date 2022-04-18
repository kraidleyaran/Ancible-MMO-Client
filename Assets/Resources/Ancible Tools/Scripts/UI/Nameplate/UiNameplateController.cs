using System.Collections.Generic;
using System.Linq;
using AncibleCoreCommon.CommonData.Client;
using AncibleCoreCommon.CommonData.Combat;
using AncibleCoreCommon.CommonData.WorldEvent;
using Assets.Ancible_Tools.Scripts.System;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Nameplate
{
    public class UiNameplateController : MonoBehaviour
    {
        private const string NAMEPLATE_FILTER = "Nameplate-Filter";

        public Vector2 Offset => _offset;

        private Vector2 _offset = Vector2.zero;
        [SerializeField] private Text _nameText;
        [SerializeField] private UiFillBarController _healthBar;
        [SerializeField] private GameObject _statusEffectGroup;
        [SerializeField] private UiObjectCastbarController _objectCastBarController;

        private GameObject _parentObj = null;

        private Dictionary<StatusEffectType, UiStatusEffectController> _statusEffects = new Dictionary<StatusEffectType, UiStatusEffectController>();

        public void Setup(GameObject obj, Vector2 offset)
        {
            _parentObj = obj;
            _offset = offset;
            var queryNetworkObjDataMsg = MessageFactory.GenerateQueryNetworkObjectDataMsg();
            queryNetworkObjDataMsg.DoAfter = RefreshData;
            gameObject.SendMessageTo(queryNetworkObjDataMsg, _parentObj);
            MessageFactory.CacheMessage(queryNetworkObjDataMsg);
            _objectCastBarController.CancelCast();
            SubscribeToObjMessages();
        }

        public void ShowCastEvent(CastWorldEvent castEvent)
        {
            _objectCastBarController.Setup(castEvent);
        }

        public void CancelCastEvent(CancelCastWorldEvent castEvent)
        {
            _objectCastBarController.CancelCast();
        }

        private void RefreshData(ClientObjectData data)
        {
            if (data != null)
            {
                _nameText.text = data.Name;
                var health = (float)data.Health / data.MaxHealth;
                if (health < 1f)
                {
                    _healthBar.DoFill(data.Health, data.MaxHealth);
                    _healthBar.gameObject.SetActive(true);
                }
                else
                {
                    _healthBar.gameObject.SetActive(false);
                }
                var statusEffects = data.StatusEffects.ToList();
                var removed = _statusEffects.Keys.Where(e => !statusEffects.Exists(s => s.Type == e)).ToArray();
                for (var i = 0; i < removed.Length; i++)
                {
                    var controller = _statusEffects[removed[i]];
                    _statusEffects.Remove(removed[i]);
                    Destroy(controller.gameObject);
                }

                for (var i = 0; i < statusEffects.Count; i++)
                {
                    if (!_statusEffects.TryGetValue(statusEffects[i].Type, out var controller))
                    {
                        controller = Instantiate(UiNameplateManager.StatusEffect, _statusEffectGroup.transform);
                        _statusEffects.Add(statusEffects[i].Type, controller);
                    }
                    controller.Setup(statusEffects[i], gameObject);
                }

                if (_statusEffects.Count > 0)
                {
                    var orderedEffects = _statusEffects.Values.OrderBy(s => s.StatusEffect).ToArray();
                    for (var i = 0; i < orderedEffects.Length; i++)
                    {
                        orderedEffects[i].transform.SetSiblingIndex(i);
                    }
                    _statusEffectGroup.gameObject.SetActive(true);
                }
                else
                {
                    ClearStatusEffects();
                }
            }
            else
            {
                _nameText.text = string.Empty;
                _healthBar.gameObject.SetActive(false);
                ClearStatusEffects();
            }

            
            

        }

        private void ClearStatusEffects()
        {
            var keys = _statusEffects.Keys.ToArray();
            for (var i = 0; i < keys.Length; i++)
            {
                var controller = _statusEffects[keys[i]];
                Destroy(controller.gameObject);
            }
            _statusEffects.Clear();
            _statusEffects = new Dictionary<StatusEffectType, UiStatusEffectController>();
            _statusEffectGroup.gameObject.SetActive(false);
        }

        void FixedUpdate()
        {
            var destination = CameraController.Camera.WorldToScreenPoint(_parentObj.transform.position.ToVector2());
            var posVector = transform.position.ToVector2();
            var destVector = destination.ToVector2();
            if (destVector != posVector)
            {
                var pos = transform.position;
                pos.x = destination.x + _offset.x;
                pos.y = destination.y + _offset.y;
                transform.position = pos;
            }
        }

        private void SubscribeToObjMessages()
        {
            //gameObject.Subscribe<UpdateTickMessage>(UpdateTick);

            _parentObj.SubscribeWithFilter<UpdateNetworkObjectDataMessage>(UpdateNetworkObjectData, NAMEPLATE_FILTER);
            _parentObj.SubscribeWithFilter<EnableObjectMessage>(EnableObject, NAMEPLATE_FILTER);
            _parentObj.SubscribeWithFilter<DisableObjectMessage>(DisableObject, NAMEPLATE_FILTER);
        }

        private void UpdateNetworkObjectData(UpdateNetworkObjectDataMessage msg)
        {
            RefreshData(msg.Data);
        }

        private void EnableObject(EnableObjectMessage msg)
        {
            var queryNetworkObjDataMsg = MessageFactory.GenerateQueryNetworkObjectDataMsg();
            queryNetworkObjDataMsg.DoAfter = RefreshData;
            gameObject.SendMessageTo(queryNetworkObjDataMsg, _parentObj);
            MessageFactory.CacheMessage(queryNetworkObjDataMsg);
            gameObject.SetActive(true);

        }

        private void DisableObject(DisableObjectMessage msg)
        {
            gameObject.SetActive(false);
        }

        public void Destroy()
        {
            gameObject.UnsubscribeFromAllMessages();
            _parentObj.UnsubscribeFromAllMessagesWithFilter(NAMEPLATE_FILTER);
            _parentObj = null;
        }

        void OnDestroy()
        {
            if (_parentObj)
            {
                Destroy();
            }
        }
    }
}