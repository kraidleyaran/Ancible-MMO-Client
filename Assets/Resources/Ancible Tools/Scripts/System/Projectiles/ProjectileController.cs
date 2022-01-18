using DG.Tweening;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System.Projectiles
{
    public class ProjectileController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Rigidbody2D _rigidBody;
        [SerializeField] private float _interpolation = 1f;
        [SerializeField] private float _stopRotationDistance = 1f;
        [SerializeField] private float _destructionDistance = 1f;
        [SerializeField] private float _destructionMovementMultiplier = .5f;

        private GameObject _target = null;
        private int _ticksRemaining = 0;
        private bool _rotate = false;

        public void Setup(Projectile projectile, int time, GameObject target)
        {
            _target = target;
            _ticksRemaining = time + (int)(WorldTickController.Discrepency * time / WorldTickController.TickRate);
            _rotate = projectile.Rotate;
            //transform.SetParent(owner.transform);
            _spriteRenderer.sprite = projectile.Sprite;
            var scale = transform.localScale;
            scale.x = projectile.Scale.x;
            scale.y = projectile.Scale.y;
            transform.localScale = scale;

            //var moveTime = time * (WorldTickController.TickRate / 1000f) + (WorldTickController.Latency / 1000f);
            //if (moveTime > 0)
            //{
            //    _moveTween = transform.DOLocalMove(Vector2.zero, moveTime).SetEase(Ease.Linear).OnComplete(
            //        () =>
            //        {
            //            _moveTween = null;
            //            Destroy(gameObject);
            //        });
            //    if (projectile.Rotate)
            //    {
            //        var initialDirection = transform.localPosition.ToVector2().GetAngle(Vector2.zero);
            //        var initialRotation = transform.localRotation.eulerAngles;
            //        initialRotation.z = initialDirection;
            //        transform.localRotation = Quaternion.Euler(initialRotation);
            //        _moveTween.OnUpdate(() =>
            //        {
            //            var direction = transform.localPosition.ToVector2().GetAngle(Vector2.zero);
            //            var rotation = transform.localRotation.eulerAngles;
            //            rotation.z = direction;
            //            transform.localRotation = Quaternion.Euler(rotation);
            //        });
            //    }
            //}
            //else
            //{
            //    Destroy(gameObject);
            //}
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<WorldTickMessage>(WorldTick);
            gameObject.Subscribe<FixedUpdateTickMessage>(FixedUpdateTick);
        }

        private void WorldTick(WorldTickMessage msg)
        {
            _ticksRemaining--;
            if (_ticksRemaining < 0)
            {
                Destroy(gameObject);
            }
        }

        private void FixedUpdateTick(FixedUpdateTickMessage msg)
        {
            if (_ticksRemaining > 0)
            {
                var difference = (_target.transform.position.ToVector2() - _rigidBody.position);
                var distance = difference.magnitude;
                var travelDistance = distance / _ticksRemaining;
                if (distance > _destructionDistance)
                {
                    travelDistance = travelDistance * _destructionMovementMultiplier;

                }

                var direction = difference.normalized;
                var pos = _rigidBody.position + (direction * travelDistance);
                _rigidBody.position = Vector2.Lerp(pos, _rigidBody.position, _interpolation * Time.fixedDeltaTime);
                if (_rotate && _stopRotationDistance < distance)
                {
                    var rotationDirection = transform.position.ToVector2().GetAngle(_target.transform.position.ToVector2());
                    _rigidBody.rotation = rotationDirection;
                }

            }
            else if (_ticksRemaining == 0)
            {
                gameObject.Unsubscribe<FixedUpdateTickMessage>();
                _rigidBody.position = _target.transform.position.ToVector2();
            }
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }

}