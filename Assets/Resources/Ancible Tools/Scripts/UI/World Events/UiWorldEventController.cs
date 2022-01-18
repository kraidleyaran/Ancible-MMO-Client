using Assets.Ancible_Tools.Scripts.System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.World_Events
{
    public class UiWorldEventController : MonoBehaviour
    {
        public RectTransform RectTransform;
        [SerializeField] private Text _eventText;

        public void Setup(string text)
        {
            _eventText.text = text;
            var height = _eventText.GetHeightOfText(text);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
    }
}