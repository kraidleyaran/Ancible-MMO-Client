using AncibleCoreCommon.CommonData.Client;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Ancible_Tools.Scripts.System.WorldSelect;
using Assets.Resources.Ancible_Tools.Scripts.Server.Traits;
using DG.Tweening;
using MessageBusLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.ObjectInfo
{
    public class UiObjectIconController : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
    {
        [SerializeField] private Image _borderImage;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _cooldownImage;

        public string Id { get; private set; }

        private Tween _cooldownTween = null;
        private ServerTrait _serverTrait = null;
        private string _title = string.Empty;
        private float _currentCooldownTime = 0f;

        private bool _hovered = false;

        public void Setup(ClientObjectIconData data, int stack = 1)
        {
            Id = data.Id;
            var title = data.Title;
            var startChar = title[0];
            var upperStartChar = startChar.ToString().ToUpper();
            title = title.Remove(0, 1);
            title = title.Insert(0, upperStartChar);
            _title = title;
            
            switch (data.Type)
            {
                case ObjectIconType.Positive:
                    _borderImage.color = WorldSelectController.Friendly;
                    break;
                case ObjectIconType.Negative:
                    _borderImage.color = WorldSelectController.Enemy;
                    break;
                case ObjectIconType.Neutral:
                    _borderImage.color = Color.white;
                    break;
            }
            var serverTrait = TraitFactoryController.GetServerTraitByName(data.Icon);
            if (serverTrait)
            {
                _serverTrait = serverTrait;
                _iconImage.sprite = serverTrait.Icon;

                if (data.MaxTicks >= 0)
                {
                    var ticksRemaining = data.MaxTicks - data.Ticks;
                    var remainingTime = data.Ticks * (WorldTickController.TickRate / 1000f + WorldTickController.Discrepency) - WorldTickController.Latency / 1000f;
                    if (remainingTime > _currentCooldownTime)
                    {
                        _currentCooldownTime = remainingTime;
                        if (_cooldownTween != null)
                        {
                            if (_cooldownImage.IsActive())
                            {
                                _cooldownTween.Kill();
                            }

                            _cooldownTween = null;
                        }
                        _cooldownImage.fillAmount = (float)ticksRemaining / data.MaxTicks;
                        _cooldownImage.gameObject.SetActive(true);
                        _cooldownTween = _cooldownImage.DOFillAmount(1f, remainingTime).SetEase(Ease.Linear).OnComplete(
                            () =>
                            {
                                _cooldownTween = null;
                            });
                    }

                }
                else
                {
                    _cooldownImage.gameObject.SetActive(false);
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hovered = true;
            var setGeneralHoverInfoMsg = MessageFactory.GenerateSetGeneralHoverInfoMsg();
            setGeneralHoverInfoMsg.Title = _title;
            setGeneralHoverInfoMsg.Description = _serverTrait.GetClientDescriptor();
            setGeneralHoverInfoMsg.Icon = _serverTrait.Icon;
            setGeneralHoverInfoMsg.Owner = gameObject;
            gameObject.SendMessage(setGeneralHoverInfoMsg);
            MessageFactory.CacheMessage(setGeneralHoverInfoMsg);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hovered = false;
            var removeHoverInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
            removeHoverInfoMsg.Owner = gameObject;
            gameObject.SendMessage(removeHoverInfoMsg);
            MessageFactory.CacheMessage(removeHoverInfoMsg);
        }

        void OnDestroy()
        {
            if (_hovered)
            {
                var removeHoverInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
                removeHoverInfoMsg.Owner = gameObject;
                gameObject.SendMessage(removeHoverInfoMsg);
                MessageFactory.CacheMessage(removeHoverInfoMsg);
            }

            if (_cooldownTween != null)
            {
                if (_cooldownTween.IsActive())
                {
                    _cooldownTween.Kill();
                }

                _cooldownTween = null;
            }
        }
    }
}