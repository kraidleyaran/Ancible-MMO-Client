using AncibleCoreCommon;
using Assets.Resources.Ancible_Tools.Scripts.UI;
using Assets.Resources.Ancible_Tools.Scripts.UI.Windows;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System.SystemMenu
{
    public class UiSystemMenuWindow : UiBaseWindow
    {
        public override bool Movable => false;
        public override bool Blocking => true;

        [SerializeField] private UiConfirmExitController _confirmExitController = null;

        public void SwitchCharacter()
        {
            UiServerStatusTextController.SetText("Sending leave request to server...");
            DataController.SetWorldState(WorldState.Switching);
            gameObject.SendMessage(LeaveWorldMessage.INSTANCE);
            ClientController.SendMessageToServer(new ClientLeaveWorldRequestMessage());
        }

        public void Exit()
        {
            _confirmExitController.gameObject.SetActive(true);
        }
    }
}