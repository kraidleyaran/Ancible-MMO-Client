using System;
using DG.Tweening;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class SpriteController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _bumpTime;
        [SerializeField] private Ease _bumpEase;
        [SerializeField] private float _bumpDistance;

        private Tween _bumpTween;
        private Vector2 _baseOffset;

        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }

        public void SetScaling(Vector2 scaling)
        {
            var scale = transform.localScale;
            scale.x = scaling.x;
            scale.y = scaling.y;
            transform.localScale = scale;
        }

        public void SetOffset(Vector2 offset)
        {
            _baseOffset = offset;
            var pos = transform.localPosition;
            pos.x = offset.x;
            pos.y = offset.y;
            transform.localPosition = pos;
        }

        public void DoBump(Vector2 direction, Action doAfter = null)
        {
            if (_bumpTween != null)
            {
                if (_bumpTween.IsActive())
                {
                    _bumpTween.Kill();
                }

                _bumpTween = null;
            }

            SetOffset(_baseOffset);
            var halfTime = _bumpTime / 2f;
            var pos = (direction * _bumpDistance) + transform.localPosition.ToVector2();
            _bumpTween = transform.DOLocalMove(pos, halfTime).SetEase(_bumpEase).OnComplete(() =>
            {
                _bumpTween = transform.DOLocalMove(_baseOffset, halfTime).SetEase(_bumpEase).OnComplete(() =>
                    {
                        _bumpTween = null;
                        doAfter?.Invoke();
                    });
            });
        }
    }
}