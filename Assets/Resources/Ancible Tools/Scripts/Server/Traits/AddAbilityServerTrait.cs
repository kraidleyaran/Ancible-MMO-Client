using AncibleCoreCommon.CommonData.Traits;
using Assets.Resources.Ancible_Tools.Scripts.Server.Abilities;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Add Ability Server Trait", menuName = "Ancible Tools/Server/Traits/Abilities/Add Ability")]
    public class AddAbilityServerTrait : ServerTrait
    {
        [SerializeField] private Ability _ability;

        public override TraitData GetData()
        {
            return new AddAbilityTraitData {Ability = _ability.name, Name = name};
        }

        public override string GetClientDescriptor()
        {
            return _ability.GetClientDescription(0);
        }
    }
}