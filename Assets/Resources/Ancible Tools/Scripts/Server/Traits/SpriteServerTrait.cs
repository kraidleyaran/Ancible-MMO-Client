using AncibleCoreCommon.CommonData.Traits;
using Assets.Ancible_Tools.Scripts.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Sprite Server Trait", menuName = "Ancible Tools/Server/Traits/Sprite")]
    public class SpriteServerTrait : ServerTrait
    {
        [SerializeField] private SpriteTrait _sprite;

        public override TraitData GetData()
        {
            return new SpriteTraitData {Name = name, Sprite = _sprite.name, MaxStack = 1};
        }
    }
}