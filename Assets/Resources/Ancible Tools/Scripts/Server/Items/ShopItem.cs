using System;
using AncibleCoreCommon.CommonData.Items;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Items
{
    [Serializable]
    public class ShopItem
    {
        public Item Item;
        public int Stack;
        public int Cost;

        public ShopItemData GetData()
        {
            return new ShopItemData {Item = Item.name, Cost = Cost, Stack = Stack};
        }
    }
}