using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.System.VisualEffects
{
    public class VisualFxFactoryController : MonoBehaviour
    {
        public static VisualFxController Controller => _instance._fxTemplate;
        public static VisualFx LevelUpFx => _instance._levelUpFx;

        private static VisualFxFactoryController _instance = null;

        [SerializeField] private string _visualFxPath = string.Empty;
        [SerializeField] private VisualFxController _fxTemplate;
        [SerializeField] private VisualFx _levelUpFx;

        private Dictionary<string, VisualFx> _visualFx = new Dictionary<string, VisualFx>();

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            _visualFx = UnityEngine.Resources.LoadAll<VisualFx>(_visualFxPath).ToDictionary(v => v.name, v => v);
        }

        public static VisualFx GetVisualFxByName(string name)
        {
            if (_instance._visualFx.TryGetValue(name, out var visualFx))
            {
                return visualFx;
            }

            return null;
        }
    }
}