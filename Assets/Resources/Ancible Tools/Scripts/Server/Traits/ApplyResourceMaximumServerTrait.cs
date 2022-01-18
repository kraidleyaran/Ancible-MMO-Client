using AncibleCoreCommon.CommonData.Ability;
using AncibleCoreCommon.CommonData.Traits;
using Assets.Ancible_Tools.Scripts.System;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Apply Maximum Resource Server Trait", menuName = "Ancible Tools/Server/Traits/Combat/Resource/Apply Maximum Resource")]
    public class ApplyResourceMaximumServerTrait : ServerTrait
    {
        [SerializeField] private ResourceType _resource;
        [SerializeField] private int _amount;
        [SerializeField] private bool _permanent;

        public override TraitData GetData()
        {
            return new ApplyResourceMaximumTraitData{Resource = _resource, Amount = _amount, Name = name, Permanent = _permanent};
        }

        public override string GetClientDescriptor()
        {
            return StaticMethods.ApplyColorToText($"+{_amount} {_resource}", ColorFactoryController.GetColorFromResource(_resource));
        }
    }
}