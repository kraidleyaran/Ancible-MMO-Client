using AncibleCoreCommon;
using AncibleCoreCommon.CommonData;
using Assets.Ancible_Tools.Scripts.System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Chat
{
    public class UiChatMessageController : MonoBehaviour
    {
        public RectTransform Transform;

        [SerializeField] private Text _chatText = null;

        public void Setup(ChatMessageData message)
        {
            var text = $"{StaticMethods.ApplyColorToText(message.Owner, ColorFactoryController.ChatUser)}:{message.Message}";
            var height = _chatText.GetHeightOfText(message.Message);
            _chatText.text = text;
            Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
    }
}