using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.UI.Chat;
using Assets.Resources.Ancible_Tools.Scripts.UI.Windows;
using Assets.Resources.Ancible_Tools.Scripts.UI.World_Events;
using MessageBusLib;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.World_Tabs
{
    public class UiWorldTabManager : UiBaseWindow
    {
        public override bool Movable => false;
        public override bool IsChild => false;
        public override bool Blocking => true;

        [SerializeField] private UiChatWindowController _chatController;
        [SerializeField] private UiWorldEventManager _worldEventManager;

        

        private WorldTabState _state = WorldTabState.Chat;

        void Awake()
        {
            _chatController.WakeUp();
            _worldEventManager.WakeUp();
            _worldEventManager.gameObject.SetActive(false);
            SubscribeToMessages();
            gameObject.SetActive(false);
        }

        public void Chat()
        {
            if (_state != WorldTabState.Chat)
            {
                SetWorldTabState(WorldTabState.Chat);
            }
        }

        public void WorldEvents()
        {
            if (_state != WorldTabState.Events)
            {
                SetWorldTabState(WorldTabState.Events);
            }

        }

        private void SetWorldTabState(WorldTabState state)
        {
            switch (state)
            {
                case WorldTabState.Chat:
                    _chatController.gameObject.SetActive(true);
                    _worldEventManager.gameObject.SetActive(false);
                    break;
                case WorldTabState.Events:
                    _chatController.gameObject.SetActive(false);
                    _worldEventManager.gameObject.SetActive(true);
                    break;
            }
            _state = state;
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<UpdateWorldStateMessage>(UpdateWorldState);
        }

        private void UpdateWorldState(UpdateWorldStateMessage msg)
        {
            gameObject.SetActive(msg.State == WorldState.Active);
        }


        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}