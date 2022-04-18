using System.Collections.Generic;
using System.Linq;
using Assets.Ancible_Tools.Scripts.Traits;
using Assets.Resources.Ancible_Tools.Scripts.Server.Traits;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class TraitFactoryController : MonoBehaviour
    {
        private static TraitFactoryController _instance = null;

        [SerializeField] private string _traitPath;
        [SerializeField] private string _serverTraithPath;

        private Dictionary<string, Trait> _traits = new Dictionary<string, Trait>();
        private Dictionary<string, SpriteTrait> _sprites = new Dictionary<string, SpriteTrait>();
        private Dictionary<string, ServerTrait> _serverTraits = new Dictionary<string, ServerTrait>();
        private Dictionary<string, DialogueServerTrait> _dialogueTraits = new Dictionary<string, DialogueServerTrait>();

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            _traits = UnityEngine.Resources.LoadAll<Trait>(_traitPath).ToDictionary(t => t.name, t => t);
            _sprites = _traits.Values.Where(t => t is SpriteTrait).Select(t => t as SpriteTrait).Where(t => t).ToDictionary(t => t.name, t => t);
            _serverTraits = UnityEngine.Resources.LoadAll<ServerTrait>(_serverTraithPath).Where(t => t).ToDictionary(t => t.name, t => t);
            _dialogueTraits = _serverTraits.Values.Where(t => t is DialogueServerTrait).Select(t => t as DialogueServerTrait).Where(t => t).ToDictionary(t => t.name, t => t);
        }

        public static SpriteTrait GetSpriteTraitByName(string traitName)
        {
            if (_instance._sprites.TryGetValue(traitName, out var spriteTrait))
            {
                return spriteTrait;
            }

            return null;
        }

        public static Sprite GetServerIconByTrait(string traitName)
        {
            if (_instance._serverTraits.TryGetValue(traitName, out var trait))
            {
                return trait.Icon;
            }

            return null;
        }

        public static ServerTrait GetServerTraitByName(string traitName)
        {
            if (_instance._serverTraits.TryGetValue(traitName, out var serverTrait))
            {
                return serverTrait;
            }

            return null;
        }

        public static DialogueServerTrait GetDialogueByTraitName(string traitName)
        {
            if (_instance._dialogueTraits.TryGetValue(traitName, out var dialogue))
            {
                return dialogue;
            }

            return null;
        }
    }
}