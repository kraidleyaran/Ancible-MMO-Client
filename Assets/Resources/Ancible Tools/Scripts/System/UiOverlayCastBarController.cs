using AncibleCoreCommon;
using Assets.Resources.Ancible_Tools.Scripts.UI.Alerts;
using DG.Tweening;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class UiOverlayCastBarController : MonoBehaviour
    {
        private static UiOverlayCastBarController _instance = null;
        
        [SerializeField] private Image _fillImage;
        [SerializeField] private Text _castText;

        private Tween _fillTween = null;

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            SubscribeToMessages();
            gameObject.SetActive(false);
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<ClientUseAbilityResultMessage>(ClientUseAbilityResult);
            gameObject.Subscribe<ClientCastCancelledMessage>(ClientCastCancelled);
        }

        private void ClientUseAbilityResult(ClientUseAbilityResultMessage msg)
        {
            if (msg.Success)
            {
                var ability = AbilityFactoryController.GetAbilityFromName(msg.Ability);
                var abilityName = ability ? ability.DisplayName : msg.Ability;
                gameObject.SetActive(true);
                if (_fillTween != null)
                {
                    if (_fillTween.IsActive())
                    {
                        _fillTween.Kill();
                    }

                    _fillTween = null;
                }

                _castText.text = abilityName;
                _fillImage.fillAmount = 0f;
                GlobalCooldownController.TriggerGlobalCooldown();
                _fillTween = _fillImage.DOFillAmount(1, WorldTickController.TickRate / 1000f * msg.CastTime + (WorldTickController.Latency / 1000f)).SetEase(Ease.Linear).OnComplete(
                    () =>
                    {
                        _fillTween = null;
                        gameObject.SetActive(false);
                    });
            }

        }

        private void ClientCastCancelled(ClientCastCancelledMessage msg)
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
            UiAlertManager.ShowAlert("Cancelled");
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
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}