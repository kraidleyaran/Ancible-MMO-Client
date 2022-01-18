using System;
using System.Linq;
using AncibleCoreCommon.CommonData.Items;
using Assets.Resources.Ancible_Tools.Scripts.Server.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Items
{
    [CreateAssetMenu(fileName = "Useable Item", menuName = "Ancible Tools/Server/Items/Useable Item")]
    public class UseableItem : Item
    {
        [SerializeField] private ServerTrait[] _applyOnUse;
        [SerializeField] private int _stackOnUse = 1;

        public override ItemType Type => ItemType.Useable;

        public override ItemData GetData()
        {
            return new UseableItemData
            {
                Name = name,
                ApplyToUser = _applyOnUse.Where(t => t).Select(t => t.name).ToArray(),
                MaxStack = MaxStack,
                UseOnStack = _stackOnUse,
                SellValue = SellValue
            };
        }

        public override string GetDescription()
        {
            var description = base.GetDescription();
            var traitDescription = GetTraitDescription();
            if (!string.IsNullOrEmpty(traitDescription))
            {
                description = string.IsNullOrEmpty(description) ? traitDescription : $"{description}{Environment.NewLine}{traitDescription}";
            }

            return description;
        }

        public string GetTraitDescription()
        {
            var description = string.Empty;
            var descriptors = _applyOnUse.Where(t => t).OrderByDescending(t => t.ClientDescriptionOrder).Select(t => t.GetClientDescriptor()).Where(d => !string.IsNullOrWhiteSpace(d)).ToArray();
            for (var i = 0; i < descriptors.Length; i++)
            {
                description = i == 0 ? descriptors[i] : $"{description}{Environment.NewLine}{descriptors[i]}";
            }

            return description;
        }
    }
}