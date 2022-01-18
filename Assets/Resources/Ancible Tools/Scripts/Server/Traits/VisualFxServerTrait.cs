using AncibleCoreCommon.CommonData.Traits;
using Assets.Resources.Ancible_Tools.Scripts.System.VisualEffects;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Visual FX Server Trait", menuName = "Ancible Tools/Server/Traits/Visual FX")]
    public class VisualFxServerTrait : ServerTrait
    {
        [SerializeField] private VisualFx _fx;

        public override TraitData GetData()
        {
            return new VisualFxTraitData {MaxStack = 0, Name = name, VisualFx = _fx.name};
        }
    }
}