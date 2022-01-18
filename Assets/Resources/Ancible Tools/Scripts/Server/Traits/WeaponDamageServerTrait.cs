using AncibleCoreCommon.CommonData;
using AncibleCoreCommon.CommonData.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Weapon Damage Server Trait", menuName = "Ancible Tools/Server/Traits/Combat/Weapon Damage")]
    public class WeaponDamageServerTrait : ServerTrait
    {
        [SerializeField] private IntNumberRange _damage;

        public override TraitData GetData()
        {
            return new WeaponDamageTraitData {Damage = _damage, Name = name, MaxStack = 1};
        }

        public override string GetClientDescriptor()
        {
            return $"{_damage} Damage";
        }
    }
}