using AncibleCoreCommon.CommonData.Ability;
using AncibleCoreCommon.CommonData.Traits;
using Assets.Ancible_Tools.Scripts.System;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Add Resource Server Trait", menuName = "Ancible Tools/Server/Traits/Combat/Resource/Add Resource")]
    public class AddResourceServerTrait : ServerTrait
    {
        [SerializeField] private ResourceType _resource;
        [SerializeField] private int _amount;

        public override TraitData GetData()
        {
            return new AddResourceTraitData { Name = name, Amount = _amount, MaxStack = 0, Resource = _resource };
        }

        public override string GetClientDescriptor()
        {
            return $"Generate {_amount} {StaticMethods.ApplyColorToText($"{_resource}", ColorFactoryController.GetColorFromResource(_resource))}";
        }
    }
}