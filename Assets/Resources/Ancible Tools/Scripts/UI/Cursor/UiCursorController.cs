using AncibleCoreCommon.CommonData;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.UI
{
    public class UiCursorController : MonoBehaviour
    {
        private static UiCursorController _instance = null;

        [SerializeField] private Sprite _defaultCursor;
        [SerializeField] private Sprite _genericInteractionCursor;
        [SerializeField] private Sprite _shopInteractionCursor;
        [SerializeField] private Sprite _healerInteractionCursor;
        [SerializeField] private Sprite _dialogueInteractionCursor;
        [SerializeField] private Sprite _attackInteractionCursor;

        [SerializeField] private Vector2 _defaultHotspot = Vector2.zero;

        private GameObject _currentOwner = null;

        private WorldCursorMode _cursorMode = WorldCursorMode.Default;

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }
            Cursor.SetCursor(_defaultCursor.texture, _defaultHotspot, CursorMode.Auto);
            _instance = this;
        }

        public static void SetCursorInteraction(InteractionType interaction, GameObject owner)
        {
            _instance._currentOwner = owner;
            switch (interaction)
            {
                case InteractionType.Talk:
                    Cursor.SetCursor(_instance._dialogueInteractionCursor.texture, _instance._defaultHotspot, CursorMode.Auto);
                    break;
                case InteractionType.Shop:
                    Cursor.SetCursor(_instance._shopInteractionCursor.texture, _instance._defaultHotspot, CursorMode.Auto);
                    break;
                case InteractionType.Heal:
                    Cursor.SetCursor(_instance._healerInteractionCursor.texture, _instance._defaultHotspot, CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(_instance._genericInteractionCursor.texture, _instance._defaultHotspot, CursorMode.Auto);
                    break;
            }

            _instance._cursorMode = WorldCursorMode.Interaction;
        }

        public static void ClearCursor(GameObject owner)
        {
            if (_instance._currentOwner && _instance._currentOwner == owner && _instance._cursorMode != WorldCursorMode.Default)
            {
                _instance._cursorMode = WorldCursorMode.Default;
                Cursor.SetCursor(_instance._defaultCursor.texture, _instance._defaultHotspot, CursorMode.Auto);
            }
            
        }

        public static void SetAttackCursor(GameObject owner)
        {
            _instance._currentOwner = owner;
            if (_instance._cursorMode != WorldCursorMode.Attack)
            {
                _instance._cursorMode = WorldCursorMode.Default;
                Cursor.SetCursor(_instance._attackInteractionCursor.texture, _instance._defaultHotspot, CursorMode.Auto);
            }
            
        }
    }
}