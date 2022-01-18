using AncibleCoreCommon.CommonData.Maps;
using Assets.Ancible_Tools.Scripts.System.Maps;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server
{
    [CreateAssetMenu(fileName = "World Map", menuName = "Ancible Tools/Server/World Map")]
    public class WorldMap : ScriptableObject
    {
        public MapController MapController;
        public string DisplayName;

        public MapData GetData()
        {
            return MapController.GetMapData(name);
        }

        public MapSpawnData GetSpawnData()
        {
            return MapController.GetSpawnData(name);
        }
    }
}