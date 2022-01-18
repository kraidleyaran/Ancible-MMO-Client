using Assets.Ancible_Tools.Scripts.System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.HoverInfo
{
    public class UiGeneralInfoController : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Text _titleText;
        [SerializeField] private Text _descriptionText;
        [SerializeField] private RectTransform _content;
        [SerializeField] private float _baseHeight = 1f;
        [SerializeField] private Text _costText;
        [SerializeField] private RectTransform _costTransform;

        public void Setup(string title, string description, Sprite icon = null)
        {
            _titleText.text = title;
            var titleHeight = _titleText.GetHeightOfText(title);
            _titleText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, titleHeight);

            _descriptionText.text = description;
            var descriptionHeight = _descriptionText.GetHeightOfText(description);
            _descriptionText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, descriptionHeight);

            if (icon)
            {
                _icon.sprite = icon;
                _icon.gameObject.SetActive(true);
            }
            else
            {
                _icon.gameObject.SetActive(false);
            }

            var height = _baseHeight + descriptionHeight + titleHeight;
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            _costTransform.gameObject.SetActive(false);
        }

        public void SetupWithCost(string title, string description,int cost, Sprite icon = null)
        {
            Setup(title, description, icon);
            if (cost >= 0)
            {
                _costText.text = $"{cost:n0}";
                _costTransform.gameObject.SetActive(true);

                var titleHeight = _titleText.GetHeightOfText(title);
                var descriptionHeight = _descriptionText.GetHeightOfText(description);
                var height = _baseHeight + descriptionHeight + titleHeight + _costTransform.rect.height;
                _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
        }

        public void SetIconColor(Color color)
        {
            _icon.color = color;
        }

        public void SetPivot(Vector2 pivot)
        {
            _content.pivot = pivot;
        }
    }
}