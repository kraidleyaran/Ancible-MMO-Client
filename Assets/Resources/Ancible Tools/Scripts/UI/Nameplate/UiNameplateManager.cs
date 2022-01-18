﻿using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.UI.ObjectInfo;
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

        public static void RegisterNameplate(GameObject obj)
        {
            if (!_instance._controllers.ContainsKey(obj))
            {
                var destination = CameraController.Camera.WorldToScreenPoint(obj.transform.position.ToVector2());
                destination.x += _instance._nameplateTemplate.Offset.x;
                destination.y += _instance._nameplateTemplate.Offset.y;
                var controller = Instantiate(_instance._nameplateTemplate,destination, Quaternion.identity,_instance.transform);
                controller.Setup(obj);
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
    }
}