using System;
using AncibleCoreCommon.CommonData.Client;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Items
{
    [Serializable]
    public class ItemStack
    {
        public Item Item = null;
        public int Stack = 0;

        public ClientItemData GetClientItemData()
        {
            var itemData = new ClientItemData {Item = Item.name};
            var stack = Stack;
            if (stack > Item.MaxStack)
            {
                stack = Item.MaxStack;
            }

            itemData.Stack = stack;
            return itemData;
        }
    }
}