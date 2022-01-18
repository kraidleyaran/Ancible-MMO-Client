using Assets.Ancible_Tools.Scripts.System;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.Traits
{
    [CreateAssetMenu(fileName = "Camera Follow Trait", menuName = "Ancible Tools/Traits/Camera Follow")]
    public class CameraFollowTrait : Trait
    {
        private SetCameraPositionMessage _setCameraPositionMsg = new SetCameraPositionMessage();

        public override void SetupController(TraitController controller)
        {
            base.SetupController(controller);
            _setCameraPositionMsg.Position = _controller.transform.parent.position.ToVector2();
            _controller.gameObject.SendMessage(_setCameraPositionMsg);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            _controller.transform.parent.gameObject.SubscribeWithFilter<UpdatePositionMessage>(UpdatePosition);
        }

        private void UpdatePosition(UpdatePositionMessage msg)
        {
            _setCameraPositionMsg.Position = msg.Position;
            _controller.gameObject.SendMessage(_setCameraPositionMsg);
        }
    }
}