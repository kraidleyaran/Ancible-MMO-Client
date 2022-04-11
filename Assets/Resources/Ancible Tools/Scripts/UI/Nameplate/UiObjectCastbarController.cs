using AncibleCoreCommon.CommonData.WorldEvent;
using Assets.Ancible_Tools.Scripts.System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Nameplate
{
    public class UiObjectCastbarController : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _fillBarImage;

        private Tween _fillTween = null;

        public void Setup(CastWorldEvent castEvent)
        {
            CancelCast();
            var ability = AbilityFactoryController.GetAbilityFromName(castEvent.Ability);
            if (ability)
            {
                _iconImage.sprite = ability.Icon;
                _iconImage.gameObject.SetActive(true);
            }
            else
            {
                _iconImage.gameObject.SetActive(false);
            }

            var fillTime = castEvent.Length * (WorldTickController.TickRate / 1000f);
            if (fillTime < 0f)
            {
                fillTime = WorldTickController.TickRate / 1000f;
            }

            _fillBarImage.fillAmount = 0f;
            gameObject.SetActive(true);
            
            _fillTween = _fillBarImage.DOFillAmount(1f, fillTime).SetEase(Ease.Linear).OnComplete(() =>
            {
                _fillTween = null;
                gameObject.SetActive(false);
            });

        }

        public void CancelCast()
        {
            if (_fillTween != null)
            {
                if (_fillTween.IsActive())
                {
                    _fillTween.Kill();
                }

                _fillTween = null;
            }

            gameObject.SetActive(false);
        }
    }
}