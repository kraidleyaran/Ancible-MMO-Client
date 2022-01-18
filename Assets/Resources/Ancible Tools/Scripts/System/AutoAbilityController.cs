using System;
using AncibleCoreCommon;
using Assets.Resources.Ancible_Tools.Scripts.Server.Abilities;
using Assets.Resources.Ancible_Tools.Scripts.UI.Alerts;
using DG.Tweening;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class AutoAbilityController : MonoBehaviour
    {
        private static AutoAbilityController _instance = null;

        private Ability _currentAbility = null;

        private string _targetId = string.Empty;
        private Sequence _timerSequence = null;

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            SubscribeToMessages();
        }

        public void StartTimer()
        {
            var castTime = _currentAbility.CastTime * (WorldTickController.TickRate / 1000f);
            var cooldown = _currentAbility.Cooldown * (WorldTickController.TickRate / 1000f + WorldTickController.Discrepency);
            var global = GlobalCooldownController.Ticks * (WorldTickController.TickRate / 1000f + WorldTickController.Discrepency);
            if (global > cooldown)
            {
                cooldown = global;
            }
            var latency = WorldTickController.Latency / 1000f;
            _timerSequence = DOTween.Sequence().AppendInterval(castTime + cooldown + latency).OnComplete(() =>
            {
                _timerSequence = null;
                ClientController.SendMessageToServer(new ClientUseAbilityRequestMessage{Ability = _currentAbility.name, TargetId = _targetId});
                StartTimer();
            });
        }

        public void StopTimer()
        {
            if (_timerSequence != null)
            {
                if (_timerSequence.IsActive())
                {
                    _timerSequence.Kill();
                }

                _timerSequence = null;
            }

            _currentAbility = null;
            _targetId = string.Empty;
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<ClientCastCancelledMessage>(ClientCastCancelled);
            gameObject.Subscribe<ClientCastFailedMessage>(ClientCastFailed);
            gameObject.Subscribe<ClientUseAbilityResultMessage>(ClientUseAbilityResult);
        }

        private void ClientCastCancelled(ClientCastCancelledMessage msg)
        {
            StopTimer();
        }

        private void ClientCastFailed(ClientCastFailedMessage msg)
        {
            StopTimer();
        }

        private void ClientUseAbilityResult(ClientUseAbilityResultMessage msg)
        {
            if (!msg.Success)
            {
                if (_currentAbility && _currentAbility.name == msg.Ability)
                {
                    StopTimer();
                    Debug.Log($"Auto Ability Timer Stopped - {DateTime.Now} - {msg.Message}");
                }
                else
                {
                    UiAlertManager.ShowAlert(msg.Message);
                }
                
            }
        }

        public static void RegisterAutoAbility(Ability ability, string targetId)
        {
            if (_instance._timerSequence != null)
            {
                _instance.StopTimer();
            }
            _instance._currentAbility = ability;
            _instance._targetId = targetId;

            _instance.StartTimer();
        }

        public static void CancelCurrentAutoAbility()
        {
            if (_instance._currentAbility)
            {
                _instance.StopTimer();
            }
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}