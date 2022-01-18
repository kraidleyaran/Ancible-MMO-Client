using Assets.Ancible_Tools.Scripts.System;
using Assets.Ancible_Tools.Scripts.System.WorldSelect;
using Assets.Ancible_Tools.Scripts.Traits;
using MessageBusLib;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Unit Select Trait", menuName = "Ancible Tools/Traits/Unit Select")]
    public class UnitSelectTrait : Trait
    {
        [SerializeField] private UnitSelectController _selectTemplate;

        private UnitSelectController _selectController = null;

        private bool _selected = false;
        private bool _hovered = false;

        public override void SetupController(TraitController controller)
        {
            base.SetupController(controller);
            _selectController = Instantiate(_selectTemplate, _controller.transform.parent);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            _controller.transform.parent.gameObject.SubscribeWithFilter<SelectObjectMessage>(SelectObject, _instanceId);
            _controller.transform.parent.gameObject.SubscribeWithFilter<UnselectObjectMessage>(UnselectObject, _instanceId);
            _controller.transform.parent.gameObject.SubscribeWithFilter<HoverObjectMessage>(HoverObject, _instanceId);
            _controller.transform.parent.gameObject.SubscribeWithFilter<UnhoverObjectMessage>(UnhoverObject, _instanceId);
            _controller.transform.parent.gameObject.SubscribeWithFilter<DisableObjectMessage>(DisableObject, _instanceId);
        }

        private void SelectObject(SelectObjectMessage msg)
        {
            _selected = true;
        }

        private void UnselectObject(UnselectObjectMessage msg)
        {
            _selected = false;
        }

        private void HoverObject(HoverObjectMessage msg)
        {
            _hovered = true;
        }

        private void UnhoverObject(UnhoverObjectMessage msg)
        {
            _hovered = false;
        }

        private void DisableObject(DisableObjectMessage msg)
        {
            if (_selected)
            {
                _controller.gameObject.SendMessage(ReturnSelectedSelectorMessage.INSTANCE);
            }

            if (_hovered)
            {
                _controller.gameObject.SendMessage(ReturnHoveredSelectorMessage.INSTANCE);
            }
        }

        public override void Destroy()
        {
            Destroy(_selectController.gameObject);
            base.Destroy();
        }
    }
}