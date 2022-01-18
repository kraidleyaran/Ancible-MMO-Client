using System;
using System.Linq;
using AncibleCoreCommon.CommonData.Items;
using Assets.Resources.Ancible_Tools.Scripts.Server.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Items
{
    [CreateAssetMenu(fileName = "Equippable Item", menuName = "Ancible Tools/Server/Items/Equippable Item")]
    public class EquippableItem : Item
    {
        [SerializeField] private ServerTrait[] _applyOnEquip;
        [SerializeField] private EquippableSlot _slot;

        public override ItemType Type => ItemType.Equippable;

        public override ItemData GetData()
        {
            return new EquippableItemData
            {
                Name = name,
                ApplyOnEquip = _applyOnEquip.Where(t => t).Select(t => t.name).ToArray(),
                MaxStack = 1,
                Slot = _slot,
                SellValue = SellValue
            };
        }

        public override string GetDescription()
        {
            var description = base.GetDescription();
            description = $"{_slot}{Environment.NewLine}{description}";
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
            var descriptors = _applyOnEquip.Where(t => t).OrderByDescending(t => t.ClientDescriptionOrder).Select(t => t.GetClientDescriptor()).Where(d => !string.IsNullOrEmpty(d)).ToArray();
            for (var i = 0; i < descriptors.Length; i++)
            {
                description = i == 0 ? descriptors[i] : $"{description}{Environment.NewLine}{descriptors[i]}";
            }

            return description;
        }
    }
}