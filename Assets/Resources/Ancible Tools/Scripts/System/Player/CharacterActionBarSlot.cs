using System;
using Assets.Resources.Ancible_Tools.Scripts.UI.ActionBar;

namespace Assets.Resources.Ancible_Tools.Scripts.System.Player
{
    [Serializable]
    public class CharacterActionBarSlot
    {
        public int Slot { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public ActionItemType Type { get; set; }
    }
}