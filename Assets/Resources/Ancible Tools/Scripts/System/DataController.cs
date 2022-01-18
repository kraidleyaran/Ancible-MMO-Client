using System;
using System.IO;
using System.Linq;
using AncibleCoreCommon;
using AncibleCoreCommon.CommonData;
using AncibleCoreCommon.CommonData.Client;
using Assets.Resources.Ancible_Tools.Scripts.System.CharacterClasses;
using Assets.Resources.Ancible_Tools.Scripts.System.Player;
using Assets.Resources.Ancible_Tools.Scripts.UI.ActionBar;
using DG.Tweening;
using MessageBusLib;
using UnityEditor;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class DataController : MonoBehaviour
    {
        public static ClientCharacterInfoData[] PlayerCharacters { get; private set; }
        public static ClientCharacterData ActiveCharacter { get; private set; }
        public static CharacterSettings ActivePlayerSettings { get; private set; }
        public static WorldState WorldState { get; private set; }
        public static bool ShopOpen { get; private set; }

        private static DataController _instance = null;

        [SerializeField] private string _characterPath = string.Empty;
        [SerializeField] private string _characterSettingsName = "CharacterSettings";
        [SerializeField] private int _localSaveEveryTick = 1;
        [SerializeField] private TextAsset _defaultSettingsJson;

        private UpdateWorldStateMessage _updateWorldStateMsg = new UpdateWorldStateMessage();
        private int _currentSaveTicks = 0;

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
            Time.fixedDeltaTime = 1 / 60f;
            _instance = this;
            SubscribeToMessages();
        }

        public static void SetWorldState(WorldState state)
        {
            WorldState = state;
            _instance._updateWorldStateMsg.State = WorldState;
            _instance.SendMessage(_instance._updateWorldStateMsg);
        }

        public static void SetShopState(bool open)
        {
            ShopOpen = open;
        }

        

        private void SaveLocalData()
        {
            var characterSettings = new CharacterSettings
            {
                Character = ActiveCharacter.Name,
                ActionSlots = UiActionBarManagerWindowController.GetPlayerData()
            };
            var characterPath = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}{_characterPath}{Path.DirectorySeparatorChar}{ActiveCharacter.Name}";
            if (!Directory.Exists(characterPath))
            {
                Directory.CreateDirectory(characterPath);
            }
            var characterSettingsPath = $"{characterPath}{Path.DirectorySeparatorChar}{_characterSettingsName}.{DataExtensions.JSON}";
            if (File.Exists(characterSettingsPath))
            {
                File.Delete(characterSettingsPath);
            }
            File.AppendAllText(characterSettingsPath,AncibleUtils.ConverToJson(characterSettings));
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<ClientCharacterResultMessage>(ClientCharacterResult);
            gameObject.Subscribe<ClientCharacterUpdateMessage>(ClientCharacterUpdate);
            gameObject.Subscribe<ClientEnterWorldWithCharacterResultMessage>(ClientEnterWorldWitCharacterResult);
            gameObject.Subscribe<WorldTickMessage>(WorldTick);
            gameObject.Subscribe<UpdateInputStateMessage>(UpdateInputState);
        }

        private void ClientCharacterResult(ClientCharacterResultMessage msg)
        {
            PlayerCharacters = msg.Characters.ToArray();
            gameObject.SendMessage(RefreshPlayerCharacterListMessage.INSTANCE);
        }

        private void ClientCharacterUpdate(ClientCharacterUpdateMessage msg)
        {
            ActiveCharacter = msg.Data;
            gameObject.SendMessage(RefreshPlayerDataMessage.INSTANCE);
        }

        private void ClientEnterWorldWitCharacterResult(ClientEnterWorldWithCharacterResultMessage msg)
        {
            if (msg.Success)
            {
                var characterPath = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}{_characterPath}{Path.DirectorySeparatorChar}{msg.Data.Name}";
                var characterSettingsPath = $"{characterPath}{Path.DirectorySeparatorChar}{_characterSettingsName}.{DataExtensions.JSON}";
                if (File.Exists(characterSettingsPath))
                {
                    var json = File.ReadAllText(characterSettingsPath);
                    var settings = AncibleUtils.FromJson<CharacterSettings>(json);
                    if (settings == null)
                    {

                        var characterClass = CharacterClassFactoryController.GetClassByName(msg.Data.PlayerClass);
                        if (characterClass)
                        {
                            settings = characterClass.GenerateNewCharacterSettings(msg.Data.Name, msg.Data.Inventory);
                        }
                        else
                        {
                            var defaultSettings = AncibleUtils.FromJson<CharacterSettings>(_defaultSettingsJson.text);
                            settings = defaultSettings ?? new CharacterSettings { ActionSlots = new CharacterActionBarSlot[0] };
                            settings.Character = msg.Data.Name;
                        }
                    }
                    ActivePlayerSettings = settings;
                }
                else
                {
                    var characterClass = CharacterClassFactoryController.GetClassByName(msg.Data.PlayerClass);
                    if (characterClass)
                    {
                        ActivePlayerSettings = characterClass.GenerateNewCharacterSettings(msg.Data.Name, msg.Data.Inventory);
                    }
                    else
                    {
                        var defaultSettings = AncibleUtils.FromJson<CharacterSettings>(_defaultSettingsJson.text);
                        ActivePlayerSettings = defaultSettings ?? new CharacterSettings { ActionSlots = new CharacterActionBarSlot[0] };
                        ActivePlayerSettings.Character = msg.Data.Name;
                    }
                }
                gameObject.SendMessage(CharacterSettingsLoadedMessage.INSTANCE);
            }
        }

        private void WorldTick(WorldTickMessage msg)
        {
            if (WorldState == WorldState.Active)
            {
                _currentSaveTicks++;
                if (_currentSaveTicks >= _localSaveEveryTick)
                {
                    SaveLocalData();
                    _currentSaveTicks = 0;
                }
            }
        }

        private void UpdateInputState(UpdateInputStateMessage msg)
        {
            if (WorldState == WorldState.Active)
            {
                if (msg.Previous.LocalSave && !msg.Current.LocalSave)
                {
                    SaveLocalData();
                    _currentSaveTicks = 0;
                }
            }
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}