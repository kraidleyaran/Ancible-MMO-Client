using AncibleCoreCommon.CommonData.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Destroy Object Server Trait", menuName = "Ancible Tools/Server/Traits/Destroy Object")]
    public class DestroyObjectServerTrait : ServerTrait
    {
        public override TraitData GetData()
        {
            return new DestroyObjectTraitData{Name = name, MaxStack = 1};
        }
    }
}