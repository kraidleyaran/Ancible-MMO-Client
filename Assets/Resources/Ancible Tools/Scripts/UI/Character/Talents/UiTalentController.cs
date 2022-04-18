using AncibleCoreCommon.CommonData.Client;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.Server.Talents;
using MessageBusLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Character.Talents
{
    public class UiTalentController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Talent Talent => _talent;
        public int TemporaryPoints { get; private set; }

        [SerializeField] private Image _talentImage = null;
        [SerializeField] private Text _appliedPointsText = null;
        [SerializeField] private Talent _talent = null;
        [SerializeField] private Text _temporaryPointsText = null;
        [SerializeField] private Button _talentButton = null;

        private ClientTalentData _talentData = null;

        public void WakeUp()
        {
            Clear();
            _appliedPointsText.text = $"{0}/{_talent.Ranks.Length}";
            _talentImage.sprite = _talent.Icon;
        }

        public void Refresh(ClientTalentData talentData, Talent[] temporary)
        {
            _talentData = talentData;
            _appliedPointsText.text = $"{_talentData?.Rank + 1 ?? 0}/{_talent.Ranks.Length}";
            _talentButton.interactable = _talent.IsTalentUnlocked(temporary);
        }

        public void ApplyTemporaryPoint()
        {
            if (_talentData == null || _talentData.Rank + TemporaryPoints < _talent.Ranks.Length - 1)
            {
                TemporaryPoints++;
                _temporaryPointsText.text = $"+{TemporaryPoints}";
            }
        }

        public void RemoveTemporaryPoint()
        {
            if (TemporaryPoints > 0)
            {
                TemporaryPoints--;
                _temporaryPointsText.text = TemporaryPoints > 0 ? $"+{TemporaryPoints}" : $"{TemporaryPoints}";
            }
        }

        public void Click()
        {
            if (_talentData == null || _talentData.Rank + TemporaryPoints < _talent.Ranks.Length - 1)
            {
                var applyTalentPointMsg = MessageFactory.GenerateApplyTalentPointMsg();
                applyTalentPointMsg.Talent = _talent;
                gameObject.SendMessage(applyTalentPointMsg);
                MessageFactory.CacheMessage(applyTalentPointMsg);
            }
        }

        public ClientTalentUpgrade GetUpgradeData()
        {
            return new ClientTalentUpgrade {IncreasedRank = TemporaryPoints, Talent = _talent.name};
        }

        public void Clear()
        {
            TemporaryPoints = 0;
            _temporaryPointsText.text = string.Empty;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            var setGeneralHoverInfoMsg = MessageFactory.GenerateSetGeneralHoverInfoMsg();
            setGeneralHoverInfoMsg.Title = _talent.DisplayName;
            setGeneralHoverInfoMsg.Description = _talent.GetDescription(_talentData?.Rank ?? 0);
            setGeneralHoverInfoMsg.Icon = _talent.Icon;
            setGeneralHoverInfoMsg.Owner = gameObject;
            gameObject.SendMessage(setGeneralHoverInfoMsg);
            MessageFactory.CacheMessage(setGeneralHoverInfoMsg);
            //TODO: Get smart description?
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            var removeGeneralInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
            removeGeneralInfoMsg.Owner = gameObject;
            gameObject.SendMessage(removeGeneralInfoMsg);
            MessageFactory.CacheMessage(removeGeneralInfoMsg);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (TemporaryPoints > 0)
                {
                    var removeTalentPointMsg = MessageFactory.GenerateRemoveTalentPointMsg();
                    removeTalentPointMsg.Talent = _talent;
                    gameObject.SendMessage(removeTalentPointMsg);
                    MessageFactory.CacheMessage(removeTalentPointMsg);
                }
            }
        }
    }
}