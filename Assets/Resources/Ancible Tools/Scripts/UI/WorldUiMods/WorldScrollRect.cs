using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.WorldUiMods
{
    public class WorldScrollRect : ScrollRect
    {
        public bool Scrolling => (_verticalBar && _verticalBar.Scrolling) || (_horizontalBar && _horizontalBar.Scrolling);

        private WorldScrollBar _verticalBar;
        private WorldScrollBar _horizontalBar;

        protected override void Awake()
        {
            if (verticalScrollbar)
            {
                _verticalBar = verticalScrollbar.gameObject.GetComponent<WorldScrollBar>();
            }

            if (horizontalScrollbar)
            {
                _horizontalBar = horizontalScrollbar.gameObject.GetComponent<WorldScrollBar>();
            }
        }
    }
}