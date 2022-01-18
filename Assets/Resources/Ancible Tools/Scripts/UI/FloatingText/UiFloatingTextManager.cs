using System.Collections.Generic;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.FloatingText
{
    public class UiFloatingTextManager : MonoBehaviour
    {
        private static UiFloatingTextManager _instance = null;

        [SerializeField] private UiFloatingTextController _floatingTextTemplate;

        private List<UiFloatingTextController> _controllers = new List<UiFloatingTextController>();

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        public static void ShowFloatingText(string text, Color color, GameObject owner)
        {
            var controller = Instantiate(_instance._floatingTextTemplate, _instance.transform);
            controller.Setup(text,color, owner);
            _instance._controllers.Add(controller);
        }

        public static void FloatingTextFinished(UiFloatingTextController controller)
        {
            _instance._controllers.Remove(controller);
            Destroy(controller.gameObject);
        }
    }
}