using System.Linq;
using AncibleCoreCommon.CommonData.Client;
using Assets.Ancible_Tools.Scripts.System;
using DG.Tweening;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI
{
    public class UiCooldownController : MonoBehaviour
    {
        public bool Active => _cooldownTween != null && _cooldownTween.IsActive();

        [SerializeField] private Image _cooldownImage;

        private Tween _cooldownTween = null;
        private float _currentCooldown = 0f;

        private CooldownType _currentCooldownType = CooldownType.None;
        private string _ability = string.Empty;

        public void WakeUp()
        {
            Clear();
            SubscribeToMessages();
        }

        private void KillCooldownTween()
        {
            if (_cooldownTween != null)
            {
                if (_cooldownTween.IsActive())
                {
                    _cooldownTween.Kill();
                }

                _currentCooldown = 0f;
                _cooldownTween = null;
            }
        }

        public void SetupAbility(ClientAbilityInfoData data)
        {
            
            _ability = data.Name;
            _currentCooldownType = CooldownType.Ability;
            var time = data.CurrentCooldownTicks * (WorldTickController.TickRate / 1000f + WorldTickController.Discrepency);
            if (_cooldownTween != null)
            {
                var cooldownTime = _currentCooldown - _cooldownTween.position;
                if (cooldownTime < time)
                {
                    KillCooldownTween();
                }
                else if (time < cooldownTime)
                {
                    if (!GlobalCooldownController.Active ||  GlobalCooldownController.Ticks * (WorldTickController.TickRate / 1000f + WorldTickController.Discrepency) - GlobalCooldownController.Cooldown.position < time)
                    {
                        KillCooldownTween();
                        if (time > 0f)
                        {
                            
                            _currentCooldown = time;
                            _cooldownTween = _cooldownImage.DOFillAmount(0f, time).SetEase(Ease.Linear).OnComplete(() =>
                            {
                                _cooldownTween = null;
                                _currentCooldown = 0f;
                                _cooldownImage.gameObject.SetActive(false);
                            });
                        }
                        else
                        {
                            _currentCooldown = WorldTickController.Latency / 1000f;
                            _cooldownTween = _cooldownImage.DOFillAmount(0f, _currentCooldown).SetEase(Ease.Linear).OnComplete(() =>
                            {
                                _cooldownTween = null;
                                _currentCooldown = 0f;
                                _cooldownImage.gameObject.SetActive(false);
                            });
                        }
                    }
                }
            }

            if (_cooldownTween == null)
            {
                _currentCooldown = time;
                if (data.CurrentCooldownTicks > 0)
                {
                    if (GlobalCooldownController.Active && GlobalCooldownController.CooldownTime - GlobalCooldownController.Cooldown.position > _currentCooldown)
                    {
                        SetupGlobalCooldown(false);
                    }
                    else
                    {
                        var fill = _currentCooldown / (data.Cooldown * (WorldTickController.TickRate / 1000f + WorldTickController.Discrepency));
                        _cooldownImage.gameObject.SetActive(true);
                        _cooldownImage.fillAmount = fill;
                        _cooldownTween = _cooldownImage.DOFillAmount(0f, _currentCooldown).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            _cooldownTween = null;
                            _currentCooldown = 0f;
                            _cooldownImage.gameObject.SetActive(false);
                        });
                    }

                }
                else if (GlobalCooldownController.Active)
                {
                    SetupGlobalCooldown(false);
                }
                else
                {
                    _cooldownImage.fillAmount = 0f;
                    _currentCooldown = 0f;
                    _cooldownImage.gameObject.SetActive(false);
                }
            }
            
        }

        public void SetupGlobalCooldown(bool clearName = true)
        {
            if (clearName)
            {
                _ability = string.Empty;
                _currentCooldownType = CooldownType.Global;
            }

            var time = 0f;
            if (GlobalCooldownController.Active)
            {
                time = GlobalCooldownController.CooldownTime - GlobalCooldownController.Cooldown.position;
                if (_cooldownTween != null)
                {
                    if (_currentCooldownType != CooldownType.Global)
                    {
                        var cooldownTime = _currentCooldown - _cooldownTween.position;
                        if (cooldownTime < time)
                        {
                            KillCooldownTween();
                        }
                    }
                    //else
                    //{
                    //    KillCooldownTween();
                    //}

                }
            }

            if (_cooldownTween == null && GlobalCooldownController.Active)
            {
                //var time = GlobalCooldownController.Ticks * (WorldTickController.TickRate / 1000f + WorldTickController.Discrepency) - GlobalCooldownController.Cooldown.position;
                var fill = time - GlobalCooldownController.Cooldown.position;
                _currentCooldown = time;
                _cooldownImage.gameObject.SetActive(true);
                _cooldownImage.fillAmount = fill;
                _cooldownTween = _cooldownImage.DOFillAmount(0f, time).SetEase(Ease.Linear).OnComplete(() =>
                {
                    _cooldownTween = null;
                    _currentCooldown = 0f;
                    _cooldownImage.fillAmount = 0f;
                    _cooldownImage.gameObject.SetActive(false);
                });
            }
        }

        public void Clear()
        {
            KillCooldownTween();
            _currentCooldownType = CooldownType.None;
            _currentCooldown = 0f;
            _ability = string.Empty;
            _cooldownImage.gameObject.SetActive(false);
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<RefreshGlobalCooldownMessage>(RefreshGlobalCooldown);
            gameObject.Subscribe<RefreshPlayerDataMessage>(RefreshPlayerData);
        }

        private void RefreshGlobalCooldown(RefreshGlobalCooldownMessage msg)
        {
            if (_currentCooldownType != CooldownType.None)
            {
                SetupGlobalCooldown(string.IsNullOrEmpty(_ability));
            }
        }

        private void RefreshPlayerData(RefreshPlayerDataMessage msg)
        {
            if (_currentCooldownType != CooldownType.None)
            {
                if (!string.IsNullOrEmpty(_ability))
                {
                    var ability = DataController.ActiveCharacter.Abilities.FirstOrDefault(a => a.Name == _ability);
                    if (ability != null)
                    {
                        SetupAbility(ability);
                    }
                    else
                    {
                        Clear();
                    }
                }
            }
        }

        void OnDestroy()
        {
            if (_cooldownTween != null)
            {
                if (_cooldownTween.IsActive())
                {
                    _cooldownTween.Kill();
                }

                _cooldownTween = null;
            }
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}