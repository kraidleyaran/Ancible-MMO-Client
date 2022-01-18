using System;
using AncibleCoreCommon.CommonData;
using AncibleCoreCommon.CommonData.Items;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Items
{
    [Serializable]
    public class LootItem
    {
        [SerializeField] public Item _item;
        [SerializeField] private IntNumberRange _stack;
        [Range(0f, 1f)] [SerializeField] private float _chanceToDrop;

        public LootItemData GetData()
        {
            return new LootItemData {Item = _item.name, Stack = _stack, ChanceToDrop = _chanceToDrop};
        }
    }
}