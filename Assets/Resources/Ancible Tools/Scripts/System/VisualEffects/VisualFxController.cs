using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.System.VisualEffects
{
    public class VisualFxController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private GameObject _parent = null;
        private Vector2 _offset = Vector2.zero;

        public void Setup(VisualFx visualFx, GameObject parent = null)
        {
            _offset = visualFx.Offset;
            _parent = parent;
            var pos = transform.position;
            pos.x += _offset.x;
            pos.y += _offset.y;
            transform.position = pos;
            
            var scale = transform.localScale;
            scale.x = visualFx.Scale.x;
            scale.y = visualFx.Scale.y;
            transform.localScale = scale;

            _spriteRenderer.sortingOrder = visualFx.SortingOrder;

            _animator.runtimeAnimatorController = visualFx.RuntimeController;
            _animator.Play(0);
        }

        void FixedUpdate()
        {
            if (_parent)
            {
                var pos = transform.position;
                pos.x = _offset.x + _parent.transform.position.x;
                pos.y = _offset.y + _parent.transform.position.y;
                transform.position = pos;
            }
        }

        public void FxAnimationFinished()
        {
            Destroy(gameObject);
        }
    }
}