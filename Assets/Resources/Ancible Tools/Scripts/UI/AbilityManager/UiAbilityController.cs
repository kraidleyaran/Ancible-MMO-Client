using AncibleCoreCommon.CommonData.Client;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Ancible_Tools.Scripts.System.WorldSelect;
using Assets.Resources.Ancible_Tools.Scripts.Server.Abilities;
using MessageBusLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.AbilityManager
{
    public class UiAbilityController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _abilityIconImage;
        [SerializeField] private Text _abilityNameText;
        [SerializeField] private Text _resourceCostText;
        [SerializeField] private UiCooldownController _cooldownController;

        public Ability Ability { get; private set; }

        private bool _hovered = false;
        private int _rank = 0;

        public void Setup(ClientAbilityInfoData data)
        {
            var ability = AbilityFactoryController.GetAbilityFromName(data.Name);
            if (ability)
            {
                _rank = data.Rank;
                Ability = ability;
                _abilityIconImage.sprite = Ability.Icon;
                _abilityNameText.text = $"{Ability.DisplayName}";
                if (Ability.ResourceCosts.Length > 0)
                {
                    var resourceCostText = string.Empty;
                    if (Ability.ResourceCosts.Length > 1)
                    {
                        for (var i = 0; i < Ability.ResourceCosts.Length; i++)
                        {
                            resourceCostText = i < Ability.ResourceCosts.Length - 1 ? $"{resourceCostText}{Ability.ResourceCosts[i].Amount} {Ability.ResourceCosts[i].Type}," : $"{resourceCostText}{Ability.ResourceCosts[i].Amount} {Ability.ResourceCosts[i].Type}";
                        }
                    }
                    else
                    {
                        resourceCostText = $"{Ability.ResourceCosts[0].Amount} {Ability.ResourceCosts[0].Type}";
                    }

                    _resourceCostText.text = resourceCostText;
                }
                else
                {
                    _resourceCostText.text = string.Empty;
                }
            }
            _cooldownController.SetupAbility(data);

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hovered = true;
            if (!WorldSelectController.PositionAbility)
            {
                WorldSelectController.SetAbilityArea(Ability.Range);
            }
            var setGeneralHoverInfoMsg = MessageFactory.GenerateSetGeneralHoverInfoMsg();
            setGeneralHoverInfoMsg.Title = Ability.DisplayName;
            setGeneralHoverInfoMsg.Description = Ability.GetClientDescription(_rank);
            setGeneralHoverInfoMsg.Owner = gameObject;
            setGeneralHoverInfoMsg.Icon = Ability.Icon;
            gameObject.SendMessage(setGeneralHoverInfoMsg);
            MessageFactory.CacheMessage(setGeneralHoverInfoMsg);

            var setHoveredAbilityMsg = MessageFactory.GenerateSetHoveredAbilityMsg();
            setHoveredAbilityMsg.Controller = this;
            gameObject.SendMessage(setHoveredAbilityMsg);
            MessageFactory.CacheMessage(setHoveredAbilityMsg);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hovered = false;
            if (!WorldSelectController.PositionAbility)
            {
                WorldSelectController.ClearAbilityArea();
            }
            var removeHoverInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
            removeHoverInfoMsg.Owner = gameObject;
            gameObject.SendMessage(removeHoverInfoMsg);
            MessageFactory.CacheMessage(removeHoverInfoMsg);

            var removeHoveredAbilityMsg = MessageFactory.GenerateRemoveHoveredAbilityMsg();
            removeHoveredAbilityMsg.Controller = this;
            gameObject.SendMessage(removeHoveredAbilityMsg);
            MessageFactory.CacheMessage(removeHoveredAbilityMsg);
        }

        void OnDestroy()
        {
            if (_hovered)
            {
                if (!WorldSelectController.PositionAbility)
                {
                    WorldSelectController.ClearAbilityArea();
                }
                var removeHoverInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
                removeHoverInfoMsg.Owner = gameObject;
                gameObject.SendMessage(removeHoverInfoMsg);
                MessageFactory.CacheMessage(removeHoverInfoMsg);
            }
        }
    }
}