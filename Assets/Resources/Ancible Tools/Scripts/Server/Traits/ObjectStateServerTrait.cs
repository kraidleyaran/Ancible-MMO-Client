using AncibleCoreCommon.CommonData.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Object State Server Trait", menuName = "Ancible Tools/Server/Traits/Object State")]
    public class ObjectStateServerTrait : ServerTrait
    {
        public override TraitData GetData()
        {
            return new ObjectStateTraitData {Name = name, MaxStack = 1};
        }
    }
}