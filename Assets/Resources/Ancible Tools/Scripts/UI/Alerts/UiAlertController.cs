using Assets.Ancible_Tools.Scripts.System;
using DG.Tweening;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Alerts
{
    public class UiAlertController : MonoBehaviour
    {
        public RectTransform RectTransform;
        [SerializeField] private Text _text;
        [SerializeField] private float _fadeTime;
        [SerializeField] private float _aliveTime;

        private Tween _fadeTween = null;
        private Sequence _timerSequence = null;

        public void Setup(string text)
        {
            var height = _text.GetHeightOfText(text);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            _text.text = text;

            _timerSequence = DOTween.Sequence().AppendInterval(_aliveTime).OnComplete(() =>
            {
                _timerSequence = null;
                _fadeTween = _text.DOFade(0f, _fadeTime).SetEase(Ease.Linear).OnComplete(() =>
                {
                    _fadeTween = null;
                    var removeAlertMsg = MessageFactory.GenerateRemoveAlertMsg();
                    removeAlertMsg.Controller = this;
                    gameObject.SendMessage(removeAlertMsg);
                    MessageFactory.CacheMessage(removeAlertMsg);
                });
            });
        }

        void OnDestroy()
        {
            if (_timerSequence != null)
            {
                if (_timerSequence.IsActive())
                {
                    _timerSequence.Kill();
                }

                _timerSequence = null;
            }

            if (_fadeTween != null)
            {
                if (_fadeTween.IsActive())
                {
                    _fadeTween.Kill();
                }

                _fadeTween = null;
            }
        }
    }
}