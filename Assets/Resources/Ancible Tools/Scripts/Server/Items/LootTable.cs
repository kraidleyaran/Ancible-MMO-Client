using System.Linq;
using AncibleCoreCommon.CommonData;
using AncibleCoreCommon.CommonData.Items;
using Assets.Resources.Ancible_Tools.Scripts.Server.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Items
{
    [CreateAssetMenu(fileName = "Loot Table", menuName = "Ancible Tools/Server/Loot/Loot Table")]
    public class LootTable : ScriptableObject
    {
        [SerializeField] protected internal string _displayName;
        [SerializeField] protected internal IntNumberRange _gold;
        [SerializeField] protected internal IntNumberRange _itemCount;
        [SerializeField] protected internal LootItem[] _items;
        [SerializeField] protected internal SpriteServerTrait _sprite;

        public virtual LootTableData GetData()
        {
            return new LootTableData { Name = name, Gold = _gold, ItemDrops = _itemCount, Items = _items.Select(i => i.GetData()).ToArray(), Sprite = _sprite.name};
        }

        public virtual LootItem[] GetItems()
        {
            return _items;
        }
    }
}