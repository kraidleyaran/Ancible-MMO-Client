using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.UI;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.Traits
{
    [CreateAssetMenu(fileName = "Input Movement Trait", menuName = "Ancible Tools/Traits/Input/Input Movement")]
    public class InputMovementTrait : Trait
    {
        private SetDirectionMessage _setDirectionMsg = new SetDirectionMessage();

        private Vector2Int _prevDirection = Vector2Int.zero;

        public override void SetupController(TraitController controller)
        {
            base.SetupController(controller);
            SubscribeToMessages();
            _setDirectionMsg.Clear = true;
        }

        private void SubscribeToMessages()
        {
            _controller.gameObject.Subscribe<UpdateInputStateMessage>(UpdateInputState);
        }

        private void UpdateInputState(UpdateInputStateMessage msg)
        {
            if (!UiController.ActiveInput)
            {
                var direction = Vector2Int.zero;
                if (msg.Current.Up)
                {
                    direction.y = 1;
                }
                else if (msg.Current.Down)
                {
                    direction.y = -1;
                }
                else if (msg.Current.Left)
                {
                    direction.x = -1;
                }
                else if (msg.Current.Right)
                {
                    direction.x = 1;
                }

                if (_prevDirection != direction)
                {
                    _setDirectionMsg.Direction = direction;
                    _controller.gameObject.SendMessageTo(_setDirectionMsg, _controller.transform.parent.gameObject);
                    _prevDirection = direction;
                }
            }
        }
    }
}