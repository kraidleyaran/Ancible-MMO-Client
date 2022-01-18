using Assets.Ancible_Tools.Scripts.System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.ActionBar
{
    public class UiActionBarShortcutController : MonoBehaviour
    {
        public RectTransform RectTransform;
        public Image IconImage;

        public string Action;
        public string Id;
        public ActionItemType Type;
        public int ExistingSlot = -1;
        public bool Actionable = false;

        public void Setup(string actionName, string actionId, Sprite icon, ActionItemType type, int existingSlot, bool actionable)
        {
            Action = actionName;
            Id = actionId;
            IconImage.sprite = icon;
            Type = type;
            ExistingSlot = existingSlot;
            Actionable = actionable;
            gameObject.SetActive(true);
        }

        public void SetPivot(Vector2 pivot)
        {
            RectTransform.pivot = pivot;
        }

        public void MoveDelta(Vector2 delta)
        {
            var position = transform.position;
            position.x += delta.x;
            position.y += delta.y;
            transform.position = position;
        }

        public void Clear()
        {
            Type = ActionItemType.Empty;
            Action = string.Empty;
            IconImage.sprite = null;
            gameObject.SetActive(false);
        }
    }
}