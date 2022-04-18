using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.WorldUiMods
{
    public class WorldScrollBar : Scrollbar
    {
        public bool Scrolling { get; private set; }

        public override void OnPointerDown(PointerEventData eventData)
        {
            Scrolling = true;
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            Scrolling = false;
            base.OnPointerUp(eventData);
        }
    }
}