using System.Collections.Generic;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.Hitbox
{
    public class HitboxController : MonoBehaviour
    {
        public Collider Collider { get; private set; }

        protected internal List<GameObject> _subscribers = new List<GameObject>();
        protected internal Hitbox _hitbox = null;

        private string _filter = string.Empty;

        public static string GenerateHitboxFilter(Hitbox hitbox, CollisionLayer layer)
        {
            return $"{hitbox.name} {layer.LayerName}";
        }

        public void Setup(Hitbox hitbox, CollisionLayer layer)
        {
            _hitbox = hitbox;
            Collider = gameObject.GetComponent<Collider>();
            gameObject.layer = layer.ToLayer();
            _filter = GenerateHitboxFilter(_hitbox, layer);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            transform.parent.gameObject.SubscribeWithFilter<HitboxCheckMessage>(HitboxCheck, _filter);
            gameObject.SubscribeWithFilter<RegisterCollisionMessage>(RegisterCollision, _filter);
            gameObject.SubscribeWithFilter<UnregisterCollisionMessage>(UnregisterCollision, _filter);
        }

        private void HitboxCheck(HitboxCheckMessage msg)
        {
            msg.DoAfter.Invoke(this);
        }

        private void RegisterCollision(RegisterCollisionMessage msg)
        {
            _subscribers.Add(msg.Object);
        }

        private void UnregisterCollision(UnregisterCollisionMessage msg)
        {
            _subscribers.Remove(msg.Object);
            if (_subscribers.Count <= 0)
            {
                Destroy();
                Destroy(gameObject);
            }
        }

        public void Destroy()
        {
            transform.parent.gameObject.UnsubscribeFromAllMessagesWithFilter(_filter);
        }
    }
}