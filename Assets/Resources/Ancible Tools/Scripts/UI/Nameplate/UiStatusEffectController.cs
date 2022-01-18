using AncibleCoreCommon;
using AncibleCoreCommon.CommonData.Client;
using AncibleCoreCommon.CommonData.Combat;
using Assets.Ancible_Tools.Scripts.System;
using DG.Tweening;
using MessageBusLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Nameplate
{
    public class UiStatusEffectController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _maskImage;
        [SerializeField] private Image _fillImage;

        public StatusEffectType StatusEffect { get; private set; }

        private Tween _fillTween = null;
        private GameObject _parent = null;

        private bool _hovered = false;

        public void Setup(ClientStatusEffectData data, GameObject parent)
        {
            _parent = parent;
            StatusEffect = data.Type;
            switch (StatusEffect)
            {
                case StatusEffectType.Daze:
                    _maskImage.sprite = AbilityFactoryController.Daze;
                    _fillImage.sprite = AbilityFactoryController.Daze;
                    _fillImage.color = ColorFactoryController.Daze;
                    break;
                case StatusEffectType.Pacify:
                    _maskImage.sprite = AbilityFactoryController.Pacify;
                    _fillImage.sprite = AbilityFactoryController.Pacify;
                    _fillImage.color = ColorFactoryController.Pacify;
                    break;
                case StatusEffectType.Root:
                    _maskImage.sprite = AbilityFactoryController.Root;
                    _fillImage.sprite = AbilityFactoryController.Root;
                    _fillImage.color = ColorFactoryController.Root;
                    break;
                case StatusEffectType.Sleep:
                    _maskImage.sprite = AbilityFactoryController.Sleep;
                    _fillImage.sprite = AbilityFactoryController.Sleep;
                    _fillImage.color = ColorFactoryController.Sleep;
                    break;
            }
            if (_fillTween != null)
            {
                if (_fillTween.IsActive())
                {
                    _fillTween.Kill();
                }

                _fillTween = null;
            }
            var time = data.CurrentTick * (WorldTickController.TickRate / 1000f + WorldTickController.Discrepency);
            _fillImage.fillAmount = (float)data.CurrentTick / data.MaxTick;
            _fillTween = _fillImage.DOFillAmount(0, time).OnComplete(() =>
            {
                _fillTween = null;
                var removeStatusEffectMsg = MessageFactory.GenerateRemoveStatusEffectMessage();
                removeStatusEffectMsg.Controller = this;
                this.SendMessageTo(removeStatusEffectMsg, _parent);
                MessageFactory.CacheMessage(removeStatusEffectMsg);
            });
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hovered = true;
            var setgeneralHoverInfoMsg = MessageFactory.GenerateSetGeneralHoverInfoMsg();
            setgeneralHoverInfoMsg.Title = $"{StatusEffect.ToPastTenseEffectString()}";
            setgeneralHoverInfoMsg.Icon = _fillImage.sprite;
            setgeneralHoverInfoMsg.IconColor = _fillImage.color;
            setgeneralHoverInfoMsg.Owner = gameObject;
            gameObject.SendMessage(setgeneralHoverInfoMsg);
            MessageFactory.CacheMessage(setgeneralHoverInfoMsg);
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
            if (_fillTween != null)
            {
                if (_fillTween.IsActive())
                {
                    _fillTween.Kill();
                }

                _fillTween = null;
            }

            _parent = null;
            if (_hovered)
            {
                var removeHoverInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
                removeHoverInfoMsg.Owner = gameObject;
                gameObject.SendMessage(removeHoverInfoMsg);
                MessageFactory.CacheMessage(removeHoverInfoMsg);
            }
        }


    }
}