using AncibleCoreCommon.CommonData;
using RogueSharp;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System.Maps
{
    public class MapTile
    {
        public Vector2Int Position;
        public ICell Cell;
        public Vector2IntData Data;
        public Vector2 WorldPosition;
        public bool Blocked;
    }
}