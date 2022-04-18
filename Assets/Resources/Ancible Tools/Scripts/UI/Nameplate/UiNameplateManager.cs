using System.Collections.Generic;
using System.Runtime.InteropServices;
using AncibleCoreCommon.CommonData.WorldEvent;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.UI.ObjectInfo;
using MessageBusLib;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Nameplate
{
    public class UiNameplateManager : MonoBehaviour
    {
        public static UiStatusEffectController StatusEffect => _instance._statusEffectTemplate;
        public static UiObjectIconController ObjectIcon => _instance._objectIconTemplate;

        private static UiNameplateManager _instance = null;

        [SerializeField] private UiNameplateController _nameplateTemplate;
        [SerializeField] private UiStatusEffectController _statusEffectTemplate;
        [SerializeField] private UiObjectIconController _objectIconTemplate;
        

        private Dictionary<GameObject, UiNameplateController> _controllers = new Dictionary<GameObject, UiNameplateController>();

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        public static void RegisterNameplate(GameObject obj, Vector2 offset)
        {
            if (!_instance._controllers.ContainsKey(obj))
            {
                var destination = CameraController.Camera.WorldToScreenPoint(obj.transform.position.ToVector2());
                destination.x += offset.x;
                destination.y += offset.y;
                var controller = Instantiate(_instance._nameplateTemplate,destination, Quaternion.identity,_instance.transform);
                controller.Setup(obj, offset);
                _instance._controllers.Add(obj, controller);
            }
        }

        public static void UnregisterNameplate(GameObject obj)
        {
            if (_instance._controllers.ContainsKey(obj))
            {
                var controller = _instance._controllers[obj];
                _instance._controllers.Remove(obj);
                Destroy(controller.gameObject);
            }
        }

        public static void ShowCastEvent(CastWorldEvent castEvent)
        {
            var obj = ObjectManagerController.GetWorldObjectById(castEvent.OwnerId);
            if (obj && obj != ObjectManagerController.PlayerObject)
            {
                if (_instance._controllers.ContainsKey(obj))
                {
                    _instance._controllers[obj].ShowCastEvent(castEvent);
                }
            }
        }

        public static void CancelCast(CancelCastWorldEvent castEvent)
        {
            var obj = ObjectManagerController.GetWorldObjectById(castEvent.OwnerId);
            if (obj && obj != ObjectManagerController.PlayerObject)
            {
                if (_instance._controllers.ContainsKey(obj))
                {
                    _instance._controllers[obj].CancelCastEvent(castEvent);
                }
            }
        }
    }
}