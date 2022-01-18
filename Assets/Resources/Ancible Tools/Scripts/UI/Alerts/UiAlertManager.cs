using System.Collections.Generic;
using AncibleCoreCommon;
using Assets.Ancible_Tools.Scripts.System;
using MessageBusLib;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Alerts
{
    public class UiAlertManager : MonoBehaviour
    {
        private static UiAlertManager _instance = null;

        [SerializeField] private int _maxAlerts = 5;
        [SerializeField] private UiAlertController _alertTemplate;

        private List<UiAlertController> _controllers = new List<UiAlertController>();


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

        public static void ShowAlert(string alert)
        {
            var controller = Instantiate(_instance._alertTemplate, _instance.transform);
            controller.Setup(alert);
            _instance._controllers.Add(controller);
            while (_instance._controllers.Count > _instance._maxAlerts)
            {
                var removeController = _instance._controllers[0];
                _instance._controllers.Remove(removeController);
                Destroy(removeController.gameObject);
            }
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<RemoveAlertMessage>(RemoveAlert);
            gameObject.Subscribe<ClientCastFailedMessage>(ClientCastFailed);
        }

        private void RemoveAlert(RemoveAlertMessage msg)
        {
            _controllers.Remove(msg.Controller);
            Destroy(msg.Controller.gameObject);
        }

        private void ClientCastFailed(ClientCastFailedMessage msg)
        {
            ShowAlert(msg.Reason);
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}