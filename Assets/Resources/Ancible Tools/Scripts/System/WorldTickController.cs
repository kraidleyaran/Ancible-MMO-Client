using System;
using System.Collections.Generic;
using AncibleCoreCommon;
using DG.Tweening;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class WorldTickController : MonoBehaviour
    {
        public static int TickRate { get; private set; }
        public static int Latency { get; private set; }
        public static float Discrepency { get; private set; }

        private static WorldTickController _instance = null;

        private DateTime _lastTick;
        private DateTime _lastServer;
        private Sequence _globalCooldownTimer = null;

        private List<int> _latencyTicks = new List<int>();
        private List<float> _discrepencies = new List<float>();

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

        void Update()
        {
            gameObject.SendMessage(UpdateTickMessage.INSTANCE);
        }

        void FixedUpdate()
        {
            gameObject.SendMessage(FixedUpdateTickMessage.INSTANCE);
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<ClientSetTickRateMessage>(ClientSetTickRate);
            gameObject.Subscribe<ClientWorldTickMessage>(ClientWorldTick);
        }

        private void ClientSetTickRate(ClientSetTickRateMessage msg)
        {
            TickRate = msg.TickRate;
        }

        private void ClientWorldTick(ClientWorldTickMessage msg)
        {
            _discrepencies.Add((float)(msg.Server - _lastServer).TotalMilliseconds / 1000f);// - TickRate / 1000f;
            while (_discrepencies.Count > 100)
            {
                _discrepencies.RemoveAt(0);
            }
            Discrepency = _discrepencies.GetAverage();
            if (Discrepency > 100f)
            {
                Discrepency = 1f;
            }
            _lastServer = msg.Server;
            var latency = (int) (DateTime.UtcNow - msg.Server).TotalMilliseconds; //+ ClientController.TimeBetweenMessages;
            if (latency < 0)
            {
                latency = 0;
            }
            _latencyTicks.Add(latency);
            if (_latencyTicks.Count > 100)
            {
                _latencyTicks.RemoveAt(0);
            }
            Latency = _latencyTicks.GetAverage();
            if (Latency <= 0)
            {
                Latency = 1;
            }
            _lastTick = DateTime.Now;
            gameObject.SendMessage(WorldTickMessage.INSTANCE);
        }
    }
}