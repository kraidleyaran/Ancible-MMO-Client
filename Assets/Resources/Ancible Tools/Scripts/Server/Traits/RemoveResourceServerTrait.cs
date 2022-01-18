using AncibleCoreCommon.CommonData.Ability;
using AncibleCoreCommon.CommonData.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Remove Resource Server Trait", menuName = "Ancible Tools/Server/Traits/Combat/Resource/Remove Resource")]
    public class RemoveResourceServerTrait : ServerTrait
    {
        [SerializeField] private ResourceType _resource;
        [SerializeField] private int _amount;

        public override TraitData GetData()
        {
            return new RemoveResourceTraitData {Amount = _amount, Name = name, Resource = _resource};
        }
    }
}