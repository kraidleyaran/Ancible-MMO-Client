#if UNITY_EDITOR
using System.IO;
using System.Linq;
using AncibleCoreCommon.CommonData;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.Server.Abilities;
using Assets.Resources.Ancible_Tools.Scripts.Server.Items;
using Assets.Resources.Ancible_Tools.Scripts.Server.Talents;
using Assets.Resources.Ancible_Tools.Scripts.Server.Traits;
using Assets.Resources.Ancible_Tools.Scripts.System.CharacterClasses;
using FileDataLib;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server
{
    [CreateAssetMenu(fileName = "Server Export Settings", menuName = "Ancible Tools/Server/Server Export Settings")]
    public class ServerExportSettings : ScriptableObject
    {
        public string TraitFolderPath;
        public string TraitSaveDataPath;

        public string MapFolderPath;
        public string MapSaveDataPath;
        public string MapSpawnSaveDataPath;

        public string CharacterClassFolderPath;
        public string CharacterClassSaveDataPath;

        public string AbilityFolderPath;
        public string AbilitySaveDataPath;

        public string ObjectTemplateFolderPath;
        public string ObjectTemplateSavePath;

        public string ItemFolderPath;
        public string ItemSavePath;

        public string LootTableFolderPath;
        public string LootTableSavePath;

        public string WorldBonusFolderPath;
        public string WorldBonusSavePath;

        public string TalentFolderPath;
        public string TalentSavePath;

        public void ExportData()
        {
            SaveTraits();
            SaveMaps();
            SaveCharacterClasses();
            SaveAbilities();
            SaveObjectTemplates();
            SaveItems();
            SaveLootTables();
            SaveWorldBonuses();
            SaveTalents();
        }

        private void PrepareDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                var files = Directory.GetFiles(directory);
                for (var i = 0; i < files.Length; i++)
                {
                    FileData.DeleteFile(files[i]);
                }
            }
            else
            {
                Directory.CreateDirectory(directory);
            }
        }

        private void SaveTraits()
        {
            PrepareDirectory(TraitSaveDataPath);
            var traits = UnityEngine.Resources.LoadAll<ServerTrait>(TraitFolderPath).ToArray();
            var traitSuccess = 0;
            for (var i = 0; i < traits.Length; i++)
            {
                var response = FileData.SaveData($"{TraitSaveDataPath}\\{traits[i].name}.{DataExtensions.TRAIT}", traits[i].GetData());
                if (response.Success)
                {
                    traitSuccess++;
                }
                else
                {
                    Debug.LogWarning(response.HasException ? $"Exception while saving Trait {traits[i].name} - {response.Exception}" : $"Unknown error while saving Trait {traits[i].name}");
                }
            }

            Debug.Log($"Succesfully saved {traitSuccess} out of {traits.Length} Traits");
        }

        private void SaveMaps()
        {
            PrepareDirectory(MapSaveDataPath);
            PrepareDirectory(MapSpawnSaveDataPath);
            var worldMaps = UnityEngine.Resources.LoadAll<WorldMap>(MapFolderPath).ToArray();
            var mapSuccuess = 0;
            var spawnSuccess = 0;
            for (var i = 0; i < worldMaps.Length; i++)
            {
                var response = FileData.SaveData($"{MapSaveDataPath}\\{worldMaps[i].name}.{DataExtensions.WORLD_MAP}", worldMaps[i].GetData());
                if (response.Success)
                {
                    mapSuccuess++;
                }
                else
                {
                    Debug.LogWarning(response.HasException ? $"Exception while saving World Map {worldMaps[i].name} - {response.Exception}" : $"Unknown error while saving World Map {worldMaps[i].name}");
                }

                var spawnResponse = FileData.SaveData($"{MapSpawnSaveDataPath}\\{worldMaps[i].name}.{DataExtensions.OBJECT_SPAWN}", worldMaps[i].GetSpawnData());
                if (spawnResponse.Success)
                {
                    spawnSuccess++;
                }
                else
                {
                    Debug.LogWarning(response.HasException ? $"Exception while saving Spawn Map {worldMaps[i].name} - {spawnResponse.Exception}" : $"Unknown error while saving Spawn Map {worldMaps[i].name}");
                }
            }

            Debug.Log($"Succesfully saved {mapSuccuess} out of {worldMaps.Length} World Maps");
            Debug.Log($"Succesfully saved {spawnSuccess} out of {worldMaps.Length} Spawn Maps");
        }

        private void SaveCharacterClasses()
        {
            PrepareDirectory(CharacterClassSaveDataPath);
            var characterClasses = UnityEngine.Resources.LoadAll<CharacterClass>(CharacterClassFolderPath).ToArray();
            var classSuccess = 0;
            for (var i = 0; i < characterClasses.Length; i++)
            {
                var response = FileData.SaveData($"{CharacterClassSaveDataPath}\\{characterClasses[i].name}.{DataExtensions.CHARACTER_CLASS}", characterClasses[i].GetData());
                if (response.Success)
                {
                    classSuccess++;
                }
                else
                {
                    Debug.LogWarning(response.HasException ? $"Exception while saving Character Class {characterClasses[i].name} - {response.Exception}" : $"Unknown error while saving Character Class {characterClasses[i].name}");
                }
            }

            Debug.Log($"Succesfully saved {classSuccess} out of {characterClasses.Length} Character Classes");
        }

        private void SaveAbilities()
        {
            PrepareDirectory(AbilitySaveDataPath);
            var abilities = UnityEngine.Resources.LoadAll<Ability>(AbilityFolderPath).ToArray();
            var abilitiySuccess = 0;
            for (var i = 0; i < abilities.Length; i++)
            {
                var response = FileData.SaveData($"{AbilitySaveDataPath}\\{abilities[i].name}.{DataExtensions.ABILITY}", abilities[i].GetData());
                if (response.Success)
                {
                    abilitiySuccess++;
                }
                else
                {
                    Debug.LogWarning(response.HasException ? $"Exception while saving Ability {abilities[i].name} - {response.Exception}" : $"Unknown error while saving Ability {abilities[i].name}");
                }
            }

            Debug.Log($"Succesfully saved {abilitiySuccess} out of {abilities.Length} Abilities");
        }

        private void SaveObjectTemplates()
        {
            PrepareDirectory(ObjectTemplateSavePath);
            var objectTemplates = UnityEngine.Resources.LoadAll<ObjectTemplate>(ObjectTemplateFolderPath).ToArray();
            var templateSuccess = 0;
            for (var i = 0; i < objectTemplates.Length; i++)
            {
                var response = FileData.SaveData($"{ObjectTemplateSavePath}\\{objectTemplates[i].name}.{DataExtensions.OBJECT_TEMPLATE}", objectTemplates[i].GetData());
                if (response.Success)
                {
                    templateSuccess++;
                }
                else
                {
                    Debug.LogWarning(response.HasException ? $"Exception while saving Object Template {objectTemplates[i].name} - {response.Exception}" : $"Unknown error while saving Object Template {objectTemplates[i].name}");
                }
            }

            Debug.Log($"Succesfully saved {templateSuccess} out of {objectTemplates.Length} Object Templates");
        }

        private void SaveItems()
        {
            PrepareDirectory(ItemSavePath);
            var items = UnityEngine.Resources.LoadAll<Item>(ItemFolderPath).ToArray();
            var itemSuccess = 0;
            for (var i = 0; i < items.Length; i++)
            {
                var response = FileData.SaveData($"{ItemSavePath}\\{items[i].name}.{DataExtensions.ITEM}", items[i].GetData());
                if (response.Success)
                {
                    itemSuccess++;
                }
                else
                {
                    Debug.LogWarning(response.HasException ? $"Exception while saving Item {items[i].name} - {response.Exception}" : $"Unknown error while saving Item {items[i].name}");
                }
            }

            Debug.Log($"Succesfully saved {itemSuccess} out of {items.Length} Items");
        }

        private void SaveLootTables()
        {
            PrepareDirectory(LootTableSavePath);
            var lootTables = UnityEngine.Resources.LoadAll<LootTable>(LootTableFolderPath).ToArray();
            var lootTableSuccess = 0;
            for (var i = 0; i < lootTables.Length; i++)
            {
                var response = FileData.SaveData($"{LootTableSavePath}\\{lootTables[i].name}.{DataExtensions.LOOT_TABLE}", lootTables[i].GetData());
                if (response.Success)
                {
                    lootTableSuccess++;
                }
                else
                {
                    Debug.LogWarning(response.HasException ? $"Exception while saving Loot Table {lootTables[i].name} - {response.Exception}" : $"Unknown error while saving Loot Table {lootTables[i].name}");
                }
            }

            Debug.Log($"Succesfully saved {lootTableSuccess} out of {lootTables.Length} Loot Tables");
        }

        private void SaveWorldBonuses()
        {
            PrepareDirectory(WorldBonusSavePath);
            var worldBonuses = UnityEngine.Resources.LoadAll<LootTable>(WorldBonusFolderPath).ToArray();
            var worldBonusSuccess = 0;
            for (var i = 0; i < worldBonuses.Length; i++)
            {
                var response = FileData.SaveData($"{WorldBonusSavePath}\\{worldBonuses[i].name}.{DataExtensions.WORLD_BONUS}", worldBonuses[i].GetData());
                if (response.Success)
                {
                    worldBonusSuccess++;
                }
                else
                {
                    Debug.LogWarning(response.HasException ? $"Exception while saving World Bonus {worldBonuses[i].name} - {response.Exception}" : $"Unknown error while saving World Bonus {worldBonuses[i].name}");
                }
            }

            Debug.Log($"Succesfully saved {worldBonusSuccess} out of {worldBonuses.Length} World Bonuses");
        }

        private void SaveTalents()
        {
            PrepareDirectory(TalentSavePath);
            var talents = UnityEngine.Resources.LoadAll<Talent>(TalentFolderPath).ToArray();
            var talentSuccess = 0;
            for (var i = 0; i < talents.Length; i++)
            {
                var response = FileData.SaveData($"{TalentSavePath}\\{talents[i].name}.{DataExtensions.TALENT}", talents[i].GetData());
                if (response.Success)
                {
                    talentSuccess++;
                }
                else
                {
                    Debug.LogWarning(response.HasException ? $"Exception while saving Talent {talents[i].name} - {response.Exception}" : $"Unknown error while saving Talent {talents[i].name}");
                }
            }

            Debug.Log($"Succesfully saved {talentSuccess} out of {talents.Length} Talents");
        }
    }
}
#endif