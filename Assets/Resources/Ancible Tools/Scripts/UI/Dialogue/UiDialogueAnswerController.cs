using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.Dialogue;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Dialogue
{
    public class UiDialogueAnswerController : MonoBehaviour
    {
        public RectTransform RectTransform;
        [SerializeField] private Text _text = null;
        [SerializeField] private float _baseHeight = 16f;

        private DialogueData _data = null;

        public void Setup(DialogueData data)
        {
            _data = data;
            _text.text = _data.Title;
            var height = _text.GetHeightOfText(data.Title) + _baseHeight;
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        public void Click()
        {
            var showDialogueMsg = MessageFactory.GenerateShowDialogueMsg();
            showDialogueMsg.Data = _data;
            gameObject.SendMessage(showDialogueMsg);
            MessageFactory.CacheMessage(showDialogueMsg);
        }
    }
}