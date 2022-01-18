using System.Collections.Generic;
using Assets.Ancible_Tools.Scripts.System;
using MessageBusLib;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Windows
{
    public class UiWindowManager : MonoBehaviour
    {

        public static bool Hovered => _instance._hovered;
        public static bool Moving => _instance._moving;
        private static UiWindowManager _instance = null;

        [SerializeField] private RectTransform _baseWindowTransform;
        [SerializeField] private RectTransform _blockingWindowTransform;

        private Dictionary<string, UiBaseWindow> _openWindows = new Dictionary<string, UiBaseWindow>();
        private Dictionary<string, Vector2> _cachedPositions = new Dictionary<string, Vector2>();

        private UiBaseWindow _hovered = null;
        private UiBaseWindow _moving = null;

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

        public static T OpenWindow<T>(T window) where T : UiBaseWindow
        {
            if (_instance._openWindows.TryGetValue(window.name, out var openWindow))
            {
                return (T) openWindow;
            }
            else
            {
                openWindow = Instantiate(window, window.Blocking ? _instance._blockingWindowTransform : _instance._baseWindowTransform);
                openWindow.name = window.name;
                if (_instance._cachedPositions.TryGetValue(window.name, out var pos))
                {
                    openWindow.SetPosition(pos);
                }
                _instance._openWindows.Add(openWindow.name, openWindow);
                return (T) openWindow;
            }
        }

        public static void CloseWindow(UiBaseWindow window)
        {
            if (_instance._openWindows.ContainsKey(window.name))
            {
                ToggleWindow(window);
            }
        }

        public static void ToggleWindow(UiBaseWindow window)
        {
            if (_instance._openWindows.ContainsKey(window.name))
            {
                var openWindow = _instance._openWindows[window.name];
                _instance._openWindows.Remove(window.name);
                var pos = openWindow.transform.position.ToVector2();
                if (!_instance._cachedPositions.ContainsKey(window.name))
                {
                    _instance._cachedPositions.Add(window.name, Vector2.zero);
                }

                _instance._cachedPositions[window.name] = pos;
                openWindow.Destroy();
                Destroy(openWindow.gameObject);
            }
            else
            {
                var openWindow = Instantiate(window, window.Blocking ? _instance._blockingWindowTransform : _instance._baseWindowTransform);
                openWindow.name = window.name;
                if (_instance._cachedPositions.TryGetValue(window.name, out var pos))
                {
                    openWindow.SetPosition(pos);
                }
                _instance._openWindows.Add(openWindow.name, openWindow);
            }
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<UpdateInputStateMessage>(UpdateInputState);
            gameObject.Subscribe<SetHoveredWindowMessage>(SetHoveredWindow);
            gameObject.Subscribe<RemoveHoveredWindowMessage>(RemoveHoveredWindow);
        }

        private void UpdateInputState(UpdateInputStateMessage msg)
        {
            if (msg.Current.MouseLeft && msg.Current.Ctrl)
            {
                if (!_moving && _hovered && _hovered.IsChild)
                {
                    _hovered.transform.SetAsLastSibling();
                    if (_hovered.Movable)
                    {
                        _moving = _hovered;
                    }
                }
                if (_moving)
                {
                    var delta = msg.Current.MousePosition - msg.Previous.MousePosition;
                    _moving.MovePosition(delta);
                }

            }
            else if (!msg.Current.Ctrl && _moving)
            {
                _moving = null;
            }

        }

        private void SetHoveredWindow(SetHoveredWindowMessage msg)
        {
            _hovered = msg.Window;
        }

        private void RemoveHoveredWindow(RemoveHoveredWindowMessage msg)
        {
            if (_hovered && _hovered == msg.Window)
            {
                _hovered = null;
            }
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }

        


    }
}