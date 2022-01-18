using AncibleCoreCommon;
using DG.Tweening;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class GlobalCooldownController : MonoBehaviour
    {
        public static Sequence Cooldown { get; private set; }
        public static float CooldownTime { get; private set; }
        public static int Ticks { get; private set; }
        public static bool Active => Cooldown != null && Cooldown.IsActive();

        private static GlobalCooldownController _instance = null;

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

        public static void TriggerGlobalCooldown()
        {
            if (Cooldown != null)
            {
                if (Cooldown.IsActive())
                {
                    Cooldown.Kill();
                }
            }

            CooldownTime = Ticks * (WorldTickController.TickRate / 1000f + WorldTickController.Discrepency);
            Cooldown = DOTween.Sequence().AppendInterval(CooldownTime).SetEase(Ease.Linear);
            Cooldown.onComplete += () =>
            {
                Cooldown = null;
            };
            _instance.SendMessage(RefreshGlobalCooldownMessage.INSTANCE);
        }

        private void SubscribeToMessages()
        {
            //gameObject.Subscribe<ClientUseAbilityResultMessage>(ClientUseAbilityResult);
            gameObject.Subscribe<ClientEnterWorldWithCharacterResultMessage>(ClientEnterWorldResult);
        }

        private void ClientUseAbilityResult(ClientUseAbilityResultMessage msg)
        {
            if (msg.Success)
            {
                //TriggerGlobalCooldown();
            }
        }

        private void ClientEnterWorldResult(ClientEnterWorldWithCharacterResultMessage msg)
        {
            if (msg.Success)
            {
                Ticks = msg.GlobalCooldown;
            }
        }
    }
}