using System;
using AncibleCoreCommon.CommonData.Items;
using Assets.Ancible_Tools.Scripts.System;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "Ancible Tools/Server/Items/General Item")]
    public class Item : ScriptableObject
    {
        public string DisplayName;
        [TextArea(3,10)] public string Description;
        public Sprite Icon;
        public ItemRarity Rarity = ItemRarity.Common;
        public int MaxStack = 1;
        public int SellValue = 0;

        public virtual ItemType Type => ItemType.General;

        public virtual ItemData GetData()
        {
            return new ItemData {Name = name, MaxStack = MaxStack, SellValue = SellValue};
        }

        public virtual string GetDescription()
        {
            return $"{Type}{Environment.NewLine}{StaticMethods.ApplyColorToText($"{Rarity}", ColorFactoryController.GetColorFromItemRairty(Rarity))}{Environment.NewLine}{Description}";
        }
    }
}