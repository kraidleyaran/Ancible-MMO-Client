using AncibleCoreCommon.CommonData.Ability;
using AncibleCoreCommon.CommonData.Combat;
using AncibleCoreCommon.CommonData.Items;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class ColorFactoryController : MonoBehaviour
    {
        public static Color Mana => _instance._manaColor;
        public static Color Spirit => _instance._spiritColor;
        public static Color Focus => _instance._focusColor;

        public static Color Daze => _instance._dazeColor;
        public static Color Pacify => _instance._pacifyColor;
        public static Color Root => _instance._rootColor;
        public static Color Sleep => _instance._sleepColor;

        private static ColorFactoryController _instance = null;

        [Header("Resource Colors")]
        [SerializeField] private Color _manaColor = Color.white;
        [SerializeField] private Color _spiritColor = Color.white;
        [SerializeField] private Color _focusColor = Color.white;

        [Header("Status Effect Colors")]
        [SerializeField] private Color _dazeColor = Color.white;
        [SerializeField] private Color _pacifyColor = Color.white;
        [SerializeField] private Color _rootColor = Color.white;
        [SerializeField] private Color _sleepColor = Color.white;

        [Header("Item Rarity Colors")]
        [SerializeField] private Color _common = Color.white;
        [SerializeField] private Color _magic = Color.white;
        [SerializeField] private Color _rare = Color.white;
        [SerializeField] private Color _epic = Color.white;
        [SerializeField] private Color _legendary = Color.white;
        [SerializeField] private Color _artifact = Color.white;

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        public static Color GetColorFromStatusEffect(StatusEffectType type)
        {
            switch (type)
            {
                case StatusEffectType.Daze:
                    return Daze;
                case StatusEffectType.Pacify:
                    return Pacify;
                case StatusEffectType.Root:
                    return Root;
                case StatusEffectType.Sleep:
                    return Sleep;
                default:
                    return Color.white;
            }
        }

        public static Color GetColorFromResource(ResourceType resource)
        {
            switch (resource)
            {
                case ResourceType.Spirit:
                    return _instance._spiritColor;
                case ResourceType.Mana:
                    return _instance._manaColor;
                case ResourceType.Focus:
                    return _instance._focusColor;
                default:
                    return Color.white;
            }
        }

        public static Color GetColorFromItemRairty(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common:
                    return _instance._common;
                case ItemRarity.Magic:
                    return _instance._magic;
                case ItemRarity.Rare:
                    return _instance._rare;
                case ItemRarity.Epic:
                    return _instance._epic;
                case ItemRarity.Legendary:
                    return _instance._legendary;
                case ItemRarity.Artifact:
                    return _instance._artifact;
                default:
                    return Color.white;
            }
        }

    }
}