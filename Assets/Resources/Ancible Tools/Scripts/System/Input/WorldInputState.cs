using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System.Input
{
    public struct WorldInputState
    {
        public bool Up;
        public bool Down;
        public bool Left;
        public bool Right;
        public bool Inventory;
        public bool Character; 
        public bool Abilities;
        public bool[] ActionBar;
        public bool Ctrl;
        public bool Tab;
        public bool LocalSave;
        public Vector2 MousePosition;
        public bool MouseLeft;
        public bool MouseRight;
        public bool Enter;
        public bool Talents;
        public bool Escape;
    }
}