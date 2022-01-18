using Assets.Ancible_Tools.Scripts.System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.FloatingText
{
    public class UiFloatingTextController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Text _text;
        [SerializeField] private float _moveTime;
        [SerializeField] private float _moveDistance;
        [SerializeField] private float _fadeTime;
        [SerializeField] private float _interpolation;

        private GameObject _owner = null;
        private Tween _tween = null;
        private Tween _fadeTween = null;

        private Vector2 _lastKnownPosition = Vector2.zero;

        public void Setup(string text, Color color, GameObject owner)
        {
            _owner = owner;
            _text.text = text;
            _text.color = color;
            var totalTime = _moveTime + _fadeTime;
            _tween = _text.rectTransform.DOLocalMoveY(_moveDistance, totalTime).SetEase(Ease.Linear).OnComplete(() =>
            {
                _tween = null;
                _owner = null;
                UiFloatingTextManager.FloatingTextFinished(this);
            });
            _tween.OnUpdate(() =>
            {
                var fadeTime = totalTime - _tween.position;
                if (_fadeTween == null && _tween.position >= _moveTime)
                {
                    _tween.OnUpdate(() => { });
                    _fadeTween = _canvasGroup.DOFade(0f, fadeTime).OnComplete(() => { _fadeTween = null; });
                }
            });

        }

        void FixedUpdate()
        {
            if (_owner)
            {
                _lastKnownPosition = _owner.transform.position.ToVector2();
            }
            var position = CameraController.Camera.WorldToScreenPoint(_lastKnownPosition);
            transform.SetTransformPosition(position);
        }

        void OnDestroy()
        {
            if (_tween != null)
            {
                if (_tween.IsActive())
                {
                    _tween.Kill();
                }

                _tween = null;
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