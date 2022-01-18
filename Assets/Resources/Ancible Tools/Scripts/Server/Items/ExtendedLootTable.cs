using System.Linq;
using AncibleCoreCommon.CommonData.Items;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Items
{
    [CreateAssetMenu(fileName = "Extended Loot Table", menuName = "Ancible Tools/Server/Loot/Extended Loot Table")]
    public class ExtendedLootTable : LootTable
    {
        [SerializeField] private LootTable _parentTable;

        public override LootTableData GetData()
        {
            var items = _parentTable.GetItems().ToList();
            items.AddRange(_items);
            return new LootTableData
            {
                Name = name,
                Gold = _gold,
                ItemDrops = _itemCount,
                Items = items.Select(i => i.GetData()).ToArray(),
                DisplayName = _displayName,
                Sprite = _sprite.name
            };

        }
    }
}