using AncibleCoreCommon.CommonData.Traits;
using Assets.Resources.Ancible_Tools.Scripts.Server.Abilities;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Upgrade Ability Server Trait", menuName = "Ancible Tools/Server/Traits/Abilities/Upgrade Ability")]
    public class UpgradeAbilityServerTrait : ServerTrait
    {
        [SerializeField] private Ability _ability;

        public override TraitData GetData()
        {
            return new UpgradeAbilityTraitData {Ability = _ability.name, MaxStack = 1, Name = name};
        }
    }
}