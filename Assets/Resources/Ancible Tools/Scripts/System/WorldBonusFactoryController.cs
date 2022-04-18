using System.Collections.Generic;
using System.Linq;
using Assets.Resources.Ancible_Tools.Scripts.Server.WorldBonuses;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class WorldBonusFactoryController : MonoBehaviour
    {
        private static WorldBonusFactoryController _instance = null;

        private Dictionary<string, WorldBonus> _bonuses = new Dictionary<string, WorldBonus>();

        [SerializeField] private string _path;

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            _bonuses = UnityEngine.Resources.LoadAll<WorldBonus>(_path).ToDictionary(b => b.name, b => b);
            Debug.Log($"Loaded {_bonuses.Count} bonuses");
        }

        public static WorldBonus GetBonusByName(string name)
        {
            if (_instance._bonuses.TryGetValue(name, out var bonus))
            {
                return bonus;
            }

            return null;
        }
    }
}