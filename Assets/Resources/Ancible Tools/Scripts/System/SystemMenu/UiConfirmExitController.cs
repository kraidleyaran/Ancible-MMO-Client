using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System.SystemMenu
{
    public class UiConfirmExitController : MonoBehaviour
    {
        public void Yes()
        {
            Application.Quit();
        }

        public void No()
        {
            gameObject.SetActive(false);
        }
    }
}