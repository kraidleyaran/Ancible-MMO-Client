using AncibleCoreCommon.CommonData.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Casting Server Trait", menuName = "Ancible Tools/Server/Traits/Casting")]
    public class CastingServerTrait : ServerTrait
    {
        public override TraitData GetData()
        {
            return new CastingTraitData{Name = name, MaxStack = 1};
        }
    }
}