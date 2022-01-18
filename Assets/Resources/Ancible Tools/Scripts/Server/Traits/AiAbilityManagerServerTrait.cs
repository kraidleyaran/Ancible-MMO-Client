using System.Linq;
using AncibleCoreCommon.CommonData.Traits;
using Assets.Resources.Ancible_Tools.Scripts.Server.Abilities;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Ai Ability Manager Server Trait", menuName = "Ancible Tools/Server/Traits/Ai/Ai Ability Manager")]
    public class AiAbilityManagerServerTrait : ServerTrait
    {
        [SerializeField] private Ability[] _abilities;

        public override TraitData GetData()
        {
            return new AiAbilityManagerTraitData { Name = name, Abilities = _abilities.Where(a => a).Select(a => a.name).ToArray(), MaxStack = 1};
        }
    }
}