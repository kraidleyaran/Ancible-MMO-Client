using System.Collections.Generic;
using System.Linq;
using AncibleCoreCommon;
using MessageBusLib;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.System.CharacterClasses
{
    public class CharacterClassFactoryController : MonoBehaviour
    {
        public static CharacterClass[] StartingClasses => _instance._startingClasses;

        private static CharacterClassFactoryController _instance = null;

        [SerializeField] private string _characteClassFolder;

        private Dictionary<string, CharacterClass> _classes = new Dictionary<string, CharacterClass>();
        private CharacterClass[] _startingClasses = new CharacterClass[0];

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            _classes = UnityEngine.Resources.LoadAll<CharacterClass>(_characteClassFolder).ToDictionary(c => c.name, c => c);
            Debug.Log($"Loaded {_classes.Count} Character Classes");
            SubscribeToMessages();
        }

        public static CharacterClass GetClassByName(string className)
        {
            if (_instance._classes.TryGetValue(className, out var characterClass))
            {
                return characterClass;
            }

            return null;
        }

        

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<ClientLoginResultMessage>(ClientLoginResult);
        }

        private void ClientLoginResult(ClientLoginResultMessage msg)
        {
            if (msg.Success)
            {
                var startingClasses = new List<CharacterClass>();
                for (var i = 0; i < msg.StartingClasses.Length; i++)
                {
                    var characterClass = GetClassByName(msg.StartingClasses[i]);
                    if (characterClass)
                    {
                        startingClasses.Add(characterClass);
                    }
                }

                _startingClasses = startingClasses.ToArray();
            }
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}