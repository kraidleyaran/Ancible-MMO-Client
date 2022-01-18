using System.Linq;
using AncibleCoreCommon.CommonData.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Healer Server Trait", menuName = "Ancible Tools/Server/Traits/Interactables/Healer")]
    public class HealerServerTrait : ServerTrait
    {
        [SerializeField] private ServerTrait[] _applyOnInteract = new ServerTrait[0];

        public override TraitData GetData()
        {
            return new HealerTraitData
            {
                Name = name,
                MaxStack = 1,
                ApplyOnInteract = _applyOnInteract.Where(t => t).Select(t => t.name).ToArray()
            };
        }
    }
}