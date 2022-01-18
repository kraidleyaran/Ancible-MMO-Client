using System.Collections.Generic;
using System.Linq;
using Assets.Resources.Ancible_Tools.Scripts.Server;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System.Maps
{
    public class MapFactoryController : MonoBehaviour
    {
        private static MapFactoryController _instance = null;

        [SerializeField] private string _mapPath = string.Empty;

        private Dictionary<string, WorldMap> _worldMaps = new Dictionary<string, WorldMap>();

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            _worldMaps = UnityEngine.Resources.LoadAll<WorldMap>(_mapPath).ToDictionary(m => m.name, m => m);
        }


        public static WorldMap GetWorldMapByName(string mapName)
        {
            if (_instance._worldMaps.TryGetValue(mapName, out var worldMap))
            {
                return worldMap;
            }

            return null;
        }

    }
}