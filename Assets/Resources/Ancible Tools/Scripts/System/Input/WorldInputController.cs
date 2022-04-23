using System.Linq;
using MessageBusLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Ancible_Tools.Scripts.System.Input
{
    public class WorldInputController : MonoBehaviour
    {
        public static Key[] ActionBar => _instance._actionBar;

        private static WorldInputController _instance = null;

        [SerializeField] private Key _up = Key.W;
        [SerializeField] private Key _down = Key.S;
        [SerializeField] private Key _left = Key.A;
        [SerializeField] private Key _right = Key.D;
        [SerializeField] private Key _inventory = Key.I;
        [SerializeField] private Key _character = Key.C;
        [SerializeField] private Key _abilities = Key.P;
        [SerializeField] private Key[] _actionBar = new Key[0];
        [SerializeField] private Key _talent = Key.N;

        private WorldInputState _previous = new WorldInputState();

        private UpdateInputStateMessage _updateInputStateMsg = new UpdateInputStateMessage();

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<UpdateTickMessage>(UpdateTick);
        }

        private void UpdateTick(UpdateTickMessage msg)
        {
            var current = new WorldInputState
            {
                Up = Keyboard.current[_up].isPressed,
                Down = Keyboard.current[_down].isPressed,
                Left = Keyboard.current[_left].isPressed,
                Right = Keyboard.current[_right].isPressed,
                Inventory = Keyboard.current[_inventory].isPressed,
                Character = Keyboard.current[_character].isPressed,
                Abilities = Keyboard.current[_abilities].isPressed,
                Ctrl = Keyboard.current[Key.LeftCtrl].isPressed,
                Tab = Keyboard.current[Key.Tab].isPressed,
                LocalSave = Keyboard.current[Key.F5].isPressed,
                ActionBar = _actionBar.Select(k => Keyboard.current[k].isPressed).ToArray(),
                MousePosition = Mouse.current.position.ReadValue(),
                MouseLeft = Mouse.current.leftButton.isPressed,
                MouseRight = Mouse.current.rightButton.isPressed,
                Enter = Keyboard.current[Key.Enter].isPressed,
                Talents = Keyboard.current[_talent].isPressed,
                Escape = Keyboard.current[Key.Escape].isPressed
            };
            _updateInputStateMsg.Current = current;
            _updateInputStateMsg.Previous = _previous;
            gameObject.SendMessage(_updateInputStateMsg);
            _previous = current;
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}