using System.Collections.Generic;
using System.Linq;
using AncibleCoreCommon.CommonData;
using AncibleCoreCommon.CommonData.Maps;
using CreativeSpore.SuperTilemapEditor;
using RogueSharp;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System.Maps
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private STETilemap _pathingTilemap;
        [SerializeField] private STETilemap[] _spawnTilemaps;

        private Map _pathingMap = null;
        private PathFinder _pathFinder = null;
        private Vector2Int _offSet = Vector2Int.zero;

        private Dictionary<Vector2Int, MapTile> _mapTiles = new Dictionary<Vector2Int, MapTile>();

        void Awake()
        {
            _offSet = new Vector2Int(_pathingTilemap.MinGridX * -1, _pathingTilemap.MinGridY * -1);
            _pathingMap = new Map(_pathingTilemap.GridWidth, _pathingTilemap.GridHeight);
            for (var x = _pathingTilemap.MinGridX; x <= _pathingTilemap.MaxGridX; x++)
            {
                for (var y = _pathingTilemap.MinGridY; y <= _pathingTilemap.MaxGridY; y++)
                {
                    if (_pathingTilemap.GetTileData(x, y) == 0)
                    {
                        var cell = _pathingMap.GetCell(x + _offSet.x, y + _offSet.y);
                        _pathingMap.SetCellProperties(cell.X, cell.Y, true, true,true);
                        _mapTiles.Add(new Vector2Int(x,y), new MapTile
                        {
                            Position = new Vector2Int(x,y),
                            Cell = cell,
                            Data = new Vector2IntData(x,y),
                            WorldPosition = GetWorldPosition(new Vector2IntData(x,y))
                        });
                    }
                }
            }

            _pathFinder = new PathFinder(_pathingMap, 100);
        }

        public MapData GetMapData(string mapName)
        {
            var mapdata = new MapData {Name = mapName};
            mapdata.Offset = new Vector2IntData{X = _pathingTilemap.MinGridX * -1, Y = _pathingTilemap.MinGridY * -1};
            var tiles = new List<MapTileData>();

            for (var x = _pathingTilemap.MinGridX; x <= _pathingTilemap.MaxGridX; x++)
            {
                for (var y = _pathingTilemap.MinGridY; y <= _pathingTilemap.MaxGridY; y++)
                {
                    if (_pathingTilemap.GetTileData(x, y) == 0)
                    {
                        tiles.Add(new MapTileData{Position = new Vector2IntData(x,y)});
                    }
                }
            }
            mapdata.Tiles = tiles.ToArray();
            mapdata.Size = new Vector2IntData(_pathingTilemap.GridWidth, _pathingTilemap.GridHeight);

            return mapdata;
        }

        public MapSpawnData GetSpawnData(string mapName)
        {
            var spawnData = new MapSpawnData {Map = mapName};
            var spawns = new List<ObjectSpawnData>();
            for (var i = 0; i < _spawnTilemaps.Length; i++)
            {
                var spawnTilemap = _spawnTilemaps[i];
                for (var x = spawnTilemap.MinGridX; x <= spawnTilemap.MaxGridX; x++)
                {
                    for (var y = spawnTilemap.MinGridY; y <= spawnTilemap.MaxGridY; y++)
                    {
                        var tileObj = spawnTilemap.GetTileObject(x, y);
                        if (tileObj)
                        {
                            var spawnController = tileObj.GetComponent<ObjectSpawnController>();
                            if (spawnController)
                            {
                                spawns.Add(spawnController.GetData(new Vector2Int(x, y)));
                            }
                        }
                    }
                }
            }


            spawnData.Spawns = spawns.ToArray();
            return spawnData;
        }

        public Vector2 GetWorldPosition(Vector2IntData tile)
        {
            return TilemapUtils.GetGridWorldPos(_pathingTilemap, tile.X, tile.Y).ToVector2();
        }

        public MapTile[] GetPathToPosition(Vector2Int origin, Vector2Int destination)
        {
            var originTile = GetMapTile(origin);
            var destinationTile = GetMapTile(destination);
            var path = _pathFinder.TryFindShortestPath(originTile.Cell, destinationTile.Cell);
            return path.Steps.Where(s => new Vector2Int(s.X - _offSet.x, s.Y - _offSet.y) != origin &&  DoesTileExistByCell(s)).Select(GetMapTileByCell).ToArray();
        }

        public MapTile GetMapTileByCell(ICell cell)
        {
            var pos = new Vector2Int(cell.X - _offSet.x, cell.Y - _offSet.y);
            if (_mapTiles.TryGetValue(pos, out var mapTile))
            {
                return mapTile;
            }

            return null;
        }

        public bool DoesTileExist(Vector2Int position)
        {
            return _mapTiles.ContainsKey(position);
        }

        public bool DoesTileExistByCell(ICell cell)
        {
            return _mapTiles.ContainsKey(new Vector2Int(cell.X - _offSet.x, cell.Y - _offSet.y));
        }

        public bool IsTileBlocked(Vector2Int position)
        {
            var tile = GetMapTile(position);
            if (tile != null)
            {
                return tile.Blocked;
            }

            return false;
        }

        public MapTile GetMapTile(Vector2Int position)
        {
            if (_mapTiles.TryGetValue(position, out var mapTile))
            {
                return mapTile;
            }

            return null;
        }

        public MapTile[] GetMapTilesInSquareArea(Vector2Int position, int area = 1)
        {
            if (_mapTiles.TryGetValue(position, out var mapTile))
            {
                var cells = _pathingMap.GetCellsInSquare(mapTile.Cell.X, mapTile.Cell.Y, area);
                return _mapTiles.Values.Where(t => cells.FirstOrDefault(c => t.Cell.X == c.X && t.Cell.Y == c.Y) != null).ToArray();
            }

            return new MapTile[0];
        }

        public void SetBlockingTile(Vector2Int position)
        {
            if (_mapTiles.TryGetValue(position, out var mapTile))
            {
                _pathingMap.SetCellProperties(mapTile.Cell.X, mapTile.Cell.Y, false, false);
                mapTile.Blocked = true;
            }
        }

        public MapTile GetTileFromWorldPos(Vector2 worldPos)
        {
            var tilePos = TilemapUtils.GetGridPositionInt(_pathingTilemap, _pathingTilemap.transform.InverseTransformPoint(worldPos).ToVector2());
            if (_mapTiles.TryGetValue(tilePos, out var tile))
            {
                return tile;
            }

            return null;
        }
    }
}