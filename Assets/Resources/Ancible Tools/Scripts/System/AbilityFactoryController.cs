using System.Collections.Generic;
using System.Linq;
using Assets.Resources.Ancible_Tools.Scripts.Server.Abilities;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class AbilityFactoryController : MonoBehaviour
    {
        public static Ability AttackAbility => _instance._attackAbility;
        public static Sprite Mana => _instance._manaIcon;
        public static Sprite Spirit => _instance._spiritIcon;
        public static Sprite Focus => _instance._focusIcon;
        public static Sprite Daze => _instance._dazeIcon;
        public static Sprite Pacify => _instance._pacifyIcon;
        public static Sprite Root => _instance._rootIcon;
        public static Sprite Sleep => _instance._sleepIcon;

        private static AbilityFactoryController _instance = null;

        [SerializeField] private string _abilityPath = string.Empty;
        [SerializeField] private Ability _attackAbility = null;

        [Header("Resource Icon References")]
        [SerializeField] private Sprite _manaIcon;
        [SerializeField] private Sprite _spiritIcon;
        [SerializeField] private Sprite _focusIcon;

        [Header("Status Effect Icon References")]
        [SerializeField] private Sprite _dazeIcon;
        [SerializeField] private Sprite _pacifyIcon;
        [SerializeField] private Sprite _rootIcon;
        [SerializeField] private Sprite _sleepIcon;

        private Dictionary<string, Ability> _abilities = new Dictionary<string, Ability>();

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            _abilities = UnityEngine.Resources.LoadAll<Ability>(_abilityPath).ToDictionary(a => a.name, a => a);
        }


        public static Ability GetAbilityFromName(string abilityName)
        {
            if (_instance._abilities.TryGetValue(abilityName, out var ability))
            {
                return ability;
            }

            return null;
        }


        
    }
}