using AncibleCoreCommon;
using AncibleCoreCommon.CommonData.WorldEvent;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.UI.World_Events;
using MessageBusLib;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.UI
{
    public class UiDeathConfirmationController : MonoBehaviour
    {
        private static UiDeathConfirmationController _instance = null;

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            gameObject.SetActive(false);
            SubscribeToMessages();
        }

        public void Respawn()
        {
            UiServerStatusTextController.SetText("Respawning");
            ClientController.SendMessageToServer(new ClientDeathConfirmationMessage());
            gameObject.SetActive(false);
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<ClientPlayerDeadMessage>(ClientPlayerDead);
        }

        private void ClientPlayerDead(ClientPlayerDeadMessage msg)
        {
            gameObject.SetActive(true);
            //UiWorldEventManager.ShowEvent(new WorldEvent{Text = "You have perished."});
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}