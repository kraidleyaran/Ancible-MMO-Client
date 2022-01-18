using Assets.Ancible_Tools.Scripts.System;
using MessageBusLib;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.PlayerOverlay
{
    public class UiPlayerExperienceBarController : MonoBehaviour
    {
        [SerializeField] private UiFillBarController _fillBar;

        void Awake()
        {
            gameObject.SetActive(false);
            SubscribeToMessages();
        }

        private void Refresh()
        {
            if ((DataController.WorldState == WorldState.Active && DataController.ActiveCharacter != null) || DataController.ActiveCharacter != null)
            {
                _fillBar.Setup(DataController.ActiveCharacter.Experience, DataController.ActiveCharacter.NextLevelExperience, $" XP");
                gameObject.SetActive(DataController.WorldState == WorldState.Active);
            }
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<RefreshPlayerDataMessage>(RefreshPlayerData);
            gameObject.Subscribe<UpdateWorldStateMessage>(UpdateWorldState);
        }

        private void RefreshPlayerData(RefreshPlayerDataMessage msg)
        {
            Refresh();
        }

        private void UpdateWorldState(UpdateWorldStateMessage msg)
        {
            Refresh();
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}