using Assets.Ancible_Tools.Scripts.System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI
{
    public class UiFillBarController : MonoBehaviour
    {
        [SerializeField] private Image _mask;
        [SerializeField] private Image _fill;
        [SerializeField] private float _baseSize = 1f;
        [SerializeField] private Text _valueText;
        [SerializeField] private float _fillTime = 2f;

        private Tween _fillTween = null;

        public void Setup(int min, int max, string customText = "")
        {
            var percent = (float) min / max;
            var size = percent * _baseSize;
            _mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            if (_valueText)
            {
                _valueText.text = $"{min} / {max}{customText}";
            }
        }

        public void DoFill(int min, int max)
        {
            if (_fillTween != null)
            {
                if (_fillTween.IsActive())
                {
                    _fillTween.Kill();
                }

                _fillTween = null;
            }
            var percent = (float)min / max;
            var horizontalSize = percent * _baseSize;
            var size = new Vector2(horizontalSize, _mask.rectTransform.sizeDelta.y);
            _fillTween = _mask.rectTransform.DOSizeDelta(size, (WorldTickController.TickRate / 1000f + WorldTickController.Latency / 1000f)).SetEase(Ease.Linear).OnComplete(() => { _fillTween = null; });
            if (_valueText)
            {
                _valueText.text = $"{min:n0} / {max:n0}";
            }
        }


        public void SetColor(Color color)
        {
            if (_fill)
            {
                _fill.color = color;
            }
        }

        void OnDestroy()
        {
            if (_fillTween != null)
            {
                if (_fillTween.IsActive())
                {
                    _fillTween.Kill();
                }

                _fillTween = null;
            }
        }
    }
}