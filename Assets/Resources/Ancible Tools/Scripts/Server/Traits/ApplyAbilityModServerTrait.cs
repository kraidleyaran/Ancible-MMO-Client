using System.Linq;
using AncibleCoreCommon.CommonData.Ability;
using AncibleCoreCommon.CommonData.Traits;
using Assets.Resources.Ancible_Tools.Scripts.Server.Abilities;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Apply Ability Mod Server Trait", menuName = "Ancible Tools/Server/Traits/Abilities/Apply Ability Mod")]
    public class ApplyAbilityModServerTrait : ServerTrait
    {
        [SerializeField] private ServerTrait[] _modTraits = new ServerTrait[0];
        [SerializeField] private AbilityModType _type = AbilityModType.Owner;
        [SerializeField] private Ability _ability = null;

        public override TraitData GetData()
        {
            return new ApplyAbilityModTraitData
            {
                Name = name,
                Mods = _modTraits.Where(t => t).Select(t => t.name).ToArray(),
                Ability = _ability.name,
                ModType = _type
            };
        }
    }
}