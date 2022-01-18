using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Resources.Ancible_Tools.Scripts.UI
{
    public class UiActiveInputFieldController : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public void OnSelect(BaseEventData eventData)
        {
            UiController.SetActiveInputField(gameObject);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            UiController.RemoveActiveInputField(gameObject);
        }

        void OnDisable()
        {
            UiController.RemoveActiveInputField(gameObject);
        }
    }
}