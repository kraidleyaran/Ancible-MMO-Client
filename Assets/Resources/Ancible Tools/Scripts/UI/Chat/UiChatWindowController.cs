using System.Collections.Generic;
using System.Linq;
using AncibleCoreCommon;
using AncibleCoreCommon.CommonData;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.UI.Windows;
using Assets.Resources.Ancible_Tools.Scripts.UI.WorldUiMods;
using DG.Tweening;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Chat
{
    public class UiChatWindowController : UiBaseWindow
    {
        public override bool Movable => false;
        public override bool IsChild => false;
        public override bool Blocking => true;

        [SerializeField] private UiChatMessageController _chatMessageTemplate;
        [SerializeField] private RectTransform _content = null;
        [SerializeField] private VerticalLayoutGroup _layoutGroup;
        [SerializeField] private int _maxMessages = 250;
        [SerializeField] private string _channel = "Global";
        [SerializeField] private WorldScrollRect _scrollView;
        [SerializeField] private UiChatInputController _chatInput = null;

        private List<UiChatMessageController> _chatMessages = new List<UiChatMessageController>();

        public void WakeUp()
        {
            _chatInput.Setup(_channel);
            SubscribeToMessages();
            gameObject.SetActive(false);
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<ClientChatUpdateMessage>(ClientChatUpdate);
            gameObject.Subscribe<UpdateWorldStateMessage>(UpdateWorldState);
        }

        private void ClientChatUpdate(ClientChatUpdateMessage msg)
        {
            for (var i = 0; i < msg.Messages.Length; i++)
            {
                var controller = Instantiate(_chatMessageTemplate, _content);
                controller.Setup(msg.Messages[i]);
                _chatMessages.Add(controller);
            }

            if (_chatMessages.Count > _maxMessages)
            {
                var deleteMessageCount = _chatMessages.Count - _maxMessages;
                var messages = _chatMessages.GetRange(0, deleteMessageCount);
                for (var i = 0; i < messages.Count; i++)
                {
                    _chatMessages.Remove(messages[i]);
                    Destroy(messages[i].gameObject);
                }
            }

            var height = _chatMessages.Sum(c => c.Transform.rect.height + _layoutGroup.spacing);
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            if (!_scrollView.Scrolling)
            {
                _scrollView.verticalScrollbar.value = 0f;
            }
        }

        private void UpdateWorldState(UpdateWorldStateMessage msg)
        {
            gameObject.SetActive(msg.State == WorldState.Active);
        }
    }
}