using AncibleCoreCommon.CommonData.Traits;
using Assets.Resources.Ancible_Tools.Scripts.Dialogue;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Dialogue Server Trait", menuName = "Ancible Tools/Server/Traits/Interactables/Dialogue")]
    public class DialogueServerTrait : ServerTrait
    {
        public DialogueData Dialogue = null;

        public override TraitData GetData()
        {
            return new DialogueTraitData {Name = name, MaxStack = _maxStack};
        }
    }
}