using System.Collections.Generic;
using AncibleCoreCommon;
using AncibleCoreCommon.CommonData;
using Assets.Ancible_Tools.Scripts.System.Maps;
using Assets.Resources.Ancible_Tools.Scripts.Server;
using Assets.Resources.Ancible_Tools.Scripts.UI;
using CreativeSpore.SuperTilemapEditor;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class WorldController : MonoBehaviour
    {
        private static WorldController _instance = null;

        [SerializeField] private STETilemap _pathing;

        private Dictionary<string, List<Vector2IntData>> _blockingTiles = new Dictionary<string, List<Vector2IntData>>();

        private MapController _currentMap = null;
        private WorldMap _mapData = null;

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            SubscribeToMessages();
        }

        public static Vector2 GetWorldPositionFromTile(Vector2IntData tile)
        {
            return TilemapUtils.GetGridWorldPos(_instance._pathing, tile.X, tile.Y).ToVector2();
        }

        public static Vector2Int GetWorldTileFromPosition(Vector2 vector)
        {
            return TilemapUtils.GetGridPositionInt(_instance._pathing, _instance.transform.InverseTransformPoint(vector));
        }

        public static bool DoesTileExistOnCurrentMap(Vector2Int position)
        {
            if (_instance._currentMap)
            {
                return _instance._currentMap.DoesTileExist(position);
            }

            return false;
        }

        public static MapTile GetTileOnCurrentMap(Vector2Int position)
        {
            if (_instance._currentMap)
            {
                return _instance._currentMap.GetMapTile(position);
            }

            return null;
        }

        public static MapTile[] GetMapTilesInSquareAreaOnCurrentMap(Vector2Int position, int area = 1)
        {
            if (_instance._currentMap)
            {
                return _instance._currentMap.GetMapTilesInSquareArea(position, area);
            }
            return new MapTile[0];
        }

        public static MapTile[] GetPathFromCurrentMap(Vector2Int origin, Vector2Int destination)
        {
            if (_instance._currentMap)
            {
                return _instance._currentMap.GetPathToPosition(origin, destination);
            }
            return new MapTile[0];
        }

        public static MapTile GetTileFromWorldPos(Vector2 worldPos)
        {
            if (_instance._currentMap)
            {
                return _instance._currentMap.GetTileFromWorldPos(worldPos);
            }
            return null;
        }

        public static bool IsTileBlocked(Vector2Int tile)
        {
            if (_instance._currentMap)
            {
                return _instance._currentMap.IsTileBlocked(tile);
            }

            return true;
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<ClientEnterWorldWithCharacterResultMessage>(ClientEnterWorld);
            gameObject.Subscribe<ClientPlayerRespawnMessage>(ClientPlayerRespawn);
            gameObject.Subscribe<ClientObjectUpdateMessage>(ClientObjectUpdate);
            gameObject.Subscribe<ClientTransferToMapMessage>(ClientTransferToMap);
            gameObject.Subscribe<ClientFinishMapTransferMessage>(ClientFinishMapTransfer);
        }

        private void ClientEnterWorld(ClientEnterWorldWithCharacterResultMessage msg)
        {
            if (msg.Success)
            {
                var map = MapFactoryController.GetWorldMapByName(msg.Data.Map);
                if (_currentMap)
                {
                    Destroy(_currentMap.gameObject);
                    _currentMap = null;
                }

                _currentMap = Instantiate(map.MapController);
                _mapData = map;
            }
        }

        private void ClientPlayerRespawn(ClientPlayerRespawnMessage msg)
        {
            if (_mapData.name != msg.Map)
            {
                var map = MapFactoryController.GetWorldMapByName(msg.Map);
                if (_currentMap)
                {
                    Destroy(_currentMap.gameObject);
                    _currentMap = null;
                }

                _currentMap = Instantiate(map.MapController);
                _mapData = map;
            }
        }

        private void ClientObjectUpdate(ClientObjectUpdateMessage msg)
        {
            var tiles = msg.Blocking;
            if (!_blockingTiles.ContainsKey(_currentMap.name))
            {
                _blockingTiles.Add(_currentMap.name, new List<Vector2IntData>());
            }
            for (var i = 0; i < tiles.Length; i++)
            {
                if (!_blockingTiles[_currentMap.name].Contains(tiles[i]))
                {
                    _blockingTiles[_currentMap.name].Add(tiles[i]);
                    _currentMap.SetBlockingTile(tiles[i].ToVector());
                }
            }
        }

        private void ClientTransferToMap(ClientTransferToMapMessage msg)
        {
            var map = MapFactoryController.GetWorldMapByName(msg.Map);
            UiServerStatusTextController.SetText($"Transferring to {map.DisplayName}");
            if (_currentMap)
            {
                Destroy(_currentMap.gameObject);
                _currentMap = null;
            }

            _currentMap = Instantiate(map.MapController);
            _mapData = map;
            if (_blockingTiles.TryGetValue(map.name, out var blocking))
            {
                for (var i = 0; i < blocking.Count; i++)
                {
                    _currentMap.SetBlockingTile(blocking[i].ToVector());
                }
            }
        }

        private void ClientFinishMapTransfer(ClientFinishMapTransferMessage msg)
        {
            UiServerStatusTextController.CloseText();
        }

        

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}