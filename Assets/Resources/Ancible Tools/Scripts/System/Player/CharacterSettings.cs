using System;

namespace Assets.Resources.Ancible_Tools.Scripts.System.Player
{
    [Serializable]
    public class CharacterSettings
    {
        public string Character { get; set; }
        public CharacterActionBarSlot[] ActionSlots { get; set; }
    }
}