using System.Linq;
using AncibleCoreCommon.CommonData.Traits;
using Assets.Resources.Ancible_Tools.Scripts.Server.Items;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Shop Server Trait", menuName = "Ancible Tools/Server/Traits/Interactables/Shop")]
    public class ShopServerTrait : ServerTrait
    {
        [SerializeField] private ShopItem[] _shopItems;

        public override TraitData GetData()
        {
            return new ShopTraitData
            {
                MaxStack = 1,
                Name = name,
                ShopItems = _shopItems.Where(s => s != null).Select(s => s.GetData()).ToArray()
            };
        }
    }
}