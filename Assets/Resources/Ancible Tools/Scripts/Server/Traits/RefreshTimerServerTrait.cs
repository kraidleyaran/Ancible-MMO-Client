using AncibleCoreCommon.CommonData.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Refresh Timer Server Trait", menuName = "Ancible Tools/Server/Traits/Combat/Time/Refresh Timer")]
    public class RefreshTimerServerTrait : ServerTrait
    {
        [SerializeField] private ServerTrait _trait;

        public override TraitData GetData()
        {
            return new RefreshTimerTraitData {Name = name, MaxStack = _maxStack, Timer = _trait.name};
        }
    }
}