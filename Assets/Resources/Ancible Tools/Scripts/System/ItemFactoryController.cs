using System.Collections.Generic;
using System.Linq;
using Assets.Resources.Ancible_Tools.Scripts.Server.Items;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class ItemFactoryController : MonoBehaviour
    {
        private static ItemFactoryController _instance = null;

        [SerializeField] private string _itemPath = string.Empty;
        

        private Dictionary<string, Item> _items = new Dictionary<string, Item>();

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            _items = UnityEngine.Resources.LoadAll<Item>(_itemPath).ToDictionary(i => i.name, i => i);
        }

        public static Item GetItemByName(string itemName)
        {
            if (_instance._items.TryGetValue(itemName, out var item))
            {
                return item;
            }

            return null;
        }

        
    }
}