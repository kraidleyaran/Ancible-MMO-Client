using System.Diagnostics;
using AncibleCoreCommon;
using Assets.Ancible_Tools.Scripts.System;
using MessageBusLib;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Chat
{
    public class UiChatInputController : InputField
    {
        private string _channel = string.Empty;

        private bool _active = false;

        public void Setup(string channel)
        {
            _channel = channel;
            SubscribeToMessages();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            _active = true;
            UiController.SetActiveInputField(gameObject);
            base.OnSelect(eventData);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            _active = false;
            UiController.RemoveActiveInputField(gameObject);
            base.OnDeselect(eventData);
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<UpdateInputStateMessage>(UpdateInputState);
        }

        private void UpdateInputState(UpdateInputStateMessage msg)
        {
            if (!msg.Previous.Enter && msg.Current.Enter && DataController.WorldState == WorldState.Active)
            {
                if (_active)
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        var cleanMessage = AncibleUtils.StripHTML(text);
                        ClientController.SendMessageToServer(new ClientChatMessage { Channel = _channel, Message = cleanMessage });
                    }
                    SetTextWithoutNotify(string.Empty);
                    if (EventSystem.current.currentSelectedGameObject == gameObject)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                    }
                }
                else
                {
                    _active = true;
                    UiController.SetActiveInputField(gameObject);
                    EventSystem.current.SetSelectedGameObject(gameObject);
                }

            }
        }
    }
}