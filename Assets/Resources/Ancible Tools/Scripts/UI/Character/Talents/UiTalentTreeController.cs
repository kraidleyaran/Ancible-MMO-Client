using System.Linq;
using AncibleCoreCommon;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.Server.Talents;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Character.Talents
{
    public class UiTalentTreeController : MonoBehaviour
    {
        [SerializeField] private UiTalentController[] _talents;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Text _availablePointsText = null;

        private int _unspentPoints = 0;
        private int _temporayPoints = 0;

        void Awake()
        {
            var playerTalents = DataController.ActiveCharacter.Talents;
            for (var i = 0; i < _talents.Length; i++)
            {
                _talents[i].WakeUp();
                var talentData = playerTalents.FirstOrDefault(t => t.Name == _talents[i].Talent.name);
                _talents[i].Refresh(talentData, new Talent[0]);
            }
            _applyButton.interactable = false;
            _unspentPoints = DataController.ActiveCharacter.UnspentTalentPoints;
            Refresh();
            SubscribeToMessages();
        }

        public void ApplyAllPoints()
        {
            //Get talent data, then clear them to make sure we're not clearing after a refresh
            _temporayPoints = 0;
            var upgrades = _talents.Where(t => t.TemporaryPoints > 0).Select(t => t.GetUpgradeData()).ToArray();
            for (var i = 0; i < _talents.Length; i++)
            {
                _talents[i].Clear();
            }

            _applyButton.interactable = false;
            if (upgrades.Length > 0)
            {
                var clientUpgradeTalentMsg = new ClientTalentUpgradeRequestMessage { Upgrades = upgrades };
                ClientController.SendMessageToServer(clientUpgradeTalentMsg);
            }
        }

        private void Refresh()
        {
            var talents = DataController.ActiveCharacter.Talents;
            var tempTalents = _talents.Where(t => t.TemporaryPoints > 0 && !talents.Any(e => e.Name == t.name)).Select(t => t.Talent).ToArray();
            for (var i = 0; i < _talents.Length; i++)
            {
                var talentData = talents.FirstOrDefault(t => t.Name == _talents[i].Talent.name);
                _talents[i].Refresh(talentData, tempTalents);
            }
            _availablePointsText.text = $"{_unspentPoints - _temporayPoints}";
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<RefreshPlayerDataMessage>(RefeshPlayerData);
            gameObject.Subscribe<ApplyTalentPointMessage>(ApplyTalentPoint);
            gameObject.Subscribe<RemoveTalentPointMessage>(RemoveTalentPoint);
        }

        private void RefeshPlayerData(RefreshPlayerDataMessage msg)
        {
            _unspentPoints = DataController.ActiveCharacter.UnspentTalentPoints;
            Refresh();
        }

        private void ApplyTalentPoint(ApplyTalentPointMessage msg)
        {
            if (_unspentPoints > 0 && _temporayPoints < _unspentPoints)
            {
                var tempUnlocked = _talents.Where(t => t.TemporaryPoints > 0).Select(t => t.Talent).ToArray();
                if (msg.Talent.IsTalentUnlocked(tempUnlocked))
                {
                    var controller = _talents.FirstOrDefault(t => t.Talent == msg.Talent);
                    if (controller)
                    {
                        controller.ApplyTemporaryPoint();
                        _temporayPoints++;
                    }
                }
            }
            _availablePointsText.text = $"{_unspentPoints - _temporayPoints}";
            _applyButton.interactable = _temporayPoints > 0;

        }

        private void RemoveTalentPoint(RemoveTalentPointMessage msg)
        {
            var controller = _talents.FirstOrDefault(c => c.Talent == msg.Talent);
            if (controller && controller.TemporaryPoints > 0)
            {
                controller.RemoveTemporaryPoint();
                _temporayPoints--;
            }

            _applyButton.interactable = _temporayPoints > 0;
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}