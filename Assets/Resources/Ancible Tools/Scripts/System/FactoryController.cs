using Assets.Ancible_Tools.Scripts.Traits;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class FactoryController : MonoBehaviour
    {
        public static UnitController UNIT_CONTROLLER { get; private set; }
        public static TraitController TRAIT_CONTROLLER { get; private set; }
        public static SpriteController SPRITE_CONTROLLER => _instance._spriteControllerTemplate;

        private static FactoryController _instance;

        [Header("Unit Templates")]
        public UnitController UnitControllerTemplate;
        public TraitController TraitControllerTemplate;
        [SerializeField] private SpriteController _spriteControllerTemplate;

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            SetupStatics();
        }

        protected internal virtual void SetupStatics()
        {
            UNIT_CONTROLLER = UnitControllerTemplate;
            TRAIT_CONTROLLER = TraitControllerTemplate;
        }
    }
}