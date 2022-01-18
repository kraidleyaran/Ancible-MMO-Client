using AncibleCoreCommon.CommonData.Traits;
using Assets.Ancible_Tools.Scripts.System;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Map Transfer Server Trait", menuName = "Ancible Tools/Server/Traits/Tile Events/Map Transfer")]
    public class MapTransferServerTrait : ServerTrait
    {
        [SerializeField] private WorldMap _map;
        [SerializeField] private Vector2Int _tile;

        public override TraitData GetData()
        {
            return new MapTransferTraitData {Map = _map.name, Name = name, MaxStack = 1, Position = _tile.ToData()};
        }
    }
}