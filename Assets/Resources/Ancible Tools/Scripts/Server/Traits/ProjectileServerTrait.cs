using System;
using System.Linq;
using AncibleCoreCommon.CommonData.Traits;
using Assets.Ancible_Tools.Scripts.System.Projectiles;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Projectile Server Trait", menuName = "Ancible Tools/Server/Traits/Combat/Projectile")]
    public class ProjectileServerTrait : ServerTrait
    {
        [SerializeField] private Projectile _projectile;
        [SerializeField] private int _travelTimePerDistance = 1;
        [SerializeField] private ServerTrait[] _applyOnContact;

        public override TraitData GetData()
        {
            return new ProjectileTraitData
            {
                Name = name,
                MaxStack = _maxStack,
                Projectile = _projectile.name,
                ApplyOnContact = _applyOnContact.Where(t => t).Select(t => t.name).ToArray(),
                TravelTime = _travelTimePerDistance
            };
        }

        public override string GetClientDescriptor()
        {
            var descriptor = $"Shoots a projectile that";
            var descriptors = _applyOnContact.Select(d => d.GetClientDescriptor()).Where(d => !string.IsNullOrWhiteSpace(d)).ToArray();
            if (descriptors.Length > 0)
            {
                for (var i = 0; i < descriptors.Length; i++)
                {
                    var traitDescriptor = descriptors[i];
                    var lowerChar = traitDescriptor[0];
                    traitDescriptor = traitDescriptor.Remove(0, 1);
                    traitDescriptor = traitDescriptor.Insert(0, char.ToLower(lowerChar).ToString());
                    descriptor = i < descriptors.Length - 1 ? $"{descriptor} {traitDescriptor}," : $"{descriptor} {traitDescriptor}";
                }
            }
            else
            {
                descriptor = $"{descriptors} does absolutely nothing?";
            }
            return descriptor;
        }
    }
}