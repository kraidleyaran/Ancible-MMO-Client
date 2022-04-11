using System.Collections.Generic;
using System.Linq;
using Assets.Resources.Ancible_Tools.Scripts.Server.Talents;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class TalentFactoryController : MonoBehaviour
    {
        private static TalentFactoryController _instance = null;

        [SerializeField] private string _talentsPath = string.Empty;

        private Dictionary<string, Talent> _talents = new Dictionary<string, Talent>();

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            _talents = UnityEngine.Resources.LoadAll<Talent>(_talentsPath).ToDictionary(t => t.name, t => t);
            Debug.Log($"Loaded {_talents.Count} Talents");
        }

        public static Talent GetTalentByName(string name)
        {
            if (_instance._talents.TryGetValue(name, out var talent))
            {
                return talent;
            }

            return null;
        }
    }
}