using Assets.Ancible_Tools.Scripts.System;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.Traits
{
    [CreateAssetMenu(fileName = "Sprite Trait", menuName = "Ancible Tools/Traits/Sprite")]
    public class SpriteTrait : Trait
    {
        public Sprite Icon => _sprite;
        public Vector2 NameplateOffset => _nameplateOffset;
        public Vector2 Offset => _offset;

        [SerializeField] private Sprite _sprite;
        [SerializeField] private Vector2 _scaling = Vector2.one;
        [SerializeField] private Vector2 _offset = Vector2.zero;
        [SerializeField] private Vector2 _nameplateOffset = Vector2.zero;

        private SpriteController _spriteController = null;

        public override void SetupController(TraitController controller)
        {
            base.SetupController(controller);
            _spriteController = Instantiate(FactoryController.SPRITE_CONTROLLER, _controller.transform.parent);
            _spriteController.SetSprite(_sprite);
            _spriteController.SetScaling(_scaling);
            _spriteController.SetOffset(_offset);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            _controller.transform.parent.gameObject.SubscribeWithFilter<DoBumpMessage>(DoBump, _instanceId);
            _controller.transform.parent.gameObject.SubscribeWithFilter<QuerySpriteMessage>(QuerySprite, _instanceId);
        }

        private void DoBump(DoBumpMessage msg)
        {
            _spriteController.DoBump(msg.Direction);
        }

        private void QuerySprite(QuerySpriteMessage msg)
        {
            msg.DoAfter.Invoke(this);
        }

        public override void Destroy()
        {
            if (_spriteController)
            {
                Destroy(_spriteController.gameObject);
                _spriteController = null;
            }

            base.Destroy();
        }
    }
}