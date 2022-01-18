using UnityEngine;
using UnityEngine.UI;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class DevUiController : MonoBehaviour
    {
        [SerializeField] private Text _latencyText;
        [SerializeField] private Text _discrepencyText;

        void LateUpdate()
        {
            _latencyText.text = $"{WorldTickController.Latency}ms";
            _discrepencyText.text = $"{WorldTickController.Discrepency:F}";
        }
    }
}