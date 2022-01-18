using System.Collections.Generic;
using System.Linq;
using AncibleCoreCommon.CommonData.CharacterClasses;
using AncibleCoreCommon.CommonData.Client;
using AncibleCoreCommon.CommonData.Combat;
using AncibleCoreCommon.CommonData.Items;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Ancible_Tools.Scripts.Traits;
using Assets.Resources.Ancible_Tools.Scripts.Server.Abilities;
using Assets.Resources.Ancible_Tools.Scripts.Server.Items;
using Assets.Resources.Ancible_Tools.Scripts.Server.Talents;
using Assets.Resources.Ancible_Tools.Scripts.System.Player;
using Assets.Resources.Ancible_Tools.Scripts.UI.ActionBar;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.System.CharacterClasses
{
    [CreateAssetMenu(fileName = "Character Class", menuName = "Ancible Tools/Data/Character Class")]
    public class CharacterClass : ScriptableObject
    {
        public string DisplayName;
        public Sprite Icon;
        public SpriteTrait[] Sprites = new SpriteTrait[0];
        public CombatStats StartingStats;
        public CombatGrowthStats Growth;
        public Ability[] StartingAbilities = new Ability[0];
        public EquippableItem[] StartingEquipment = new EquippableItem[0];
        public ClientResourceData[] StartingResources = new ClientResourceData[0];
        public ItemStack[] StartingItems = new ItemStack[0];
        public ClassLevelUp[] LevelUp = new ClassLevelUp[0];
        public Talent[] Talents = new Talent[0];

        public CharacterClassData GetData()
        {
            return new CharacterClassData
            {
                Class = name,
                Sprites = Sprites.Where(s => s).Select(s => s.name).ToArray(),
                StartingStats = StartingStats,
                GrowthStats = Growth,
                StartingAbilities = StartingAbilities.Where(a => a).Select(a => a.name).ToArray(),
                StartingEquipment = StartingEquipment.Where(i => i).Select(i => i.name).ToArray(),
                StartingItems = StartingItems.Where(i => i != null).Select(i => i.GetClientItemData()).ToArray(),
                Resources = StartingResources.Where(r => r != null).ToArray(),
                LevelUpData = LevelUp.Where(l => l != null).Select(l => l.GetData()).ToArray(),
                Talents = Talents.Where(t => t).Select(t => t.name).ToArray()
            };
        }

        public CharacterSettings GenerateNewCharacterSettings(string characterName, ClientItemData[] inventory)
        {
            var characterSettings = new CharacterSettings {Character = characterName};

            var actionSlots = new List<CharacterActionBarSlot>();
            actionSlots.Add(new CharacterActionBarSlot{Name = AbilityFactoryController.AttackAbility.name, Slot = actionSlots.Count, Type = ActionItemType.Ability});
            for (var i = 0; i < StartingAbilities.Length; i++)
            {
                actionSlots.Add(new CharacterActionBarSlot{Name = StartingAbilities[i].name, Slot = actionSlots.Count, Type = ActionItemType.Ability});
            }

            var startingItems = inventory.Where(i => StartingItems.FirstOrDefault(s => s.Item.name == i.Item) != null).ToArray();
            for (var i = 0; i < startingItems.Length; i++)
            {
                if (!actionSlots.Exists(a => a.Type == ActionItemType.Item && a.Name != startingItems[i].Item) && actionSlots.Count < 10)
                {
                    actionSlots.Add(new CharacterActionBarSlot{Type = ActionItemType.Item, Name = startingItems[i].Item, Id = startingItems[i].ItemId, Slot = actionSlots.Count});
                }
            }

            characterSettings.ActionSlots = actionSlots.ToArray();
            return characterSettings;
        }
    }
}