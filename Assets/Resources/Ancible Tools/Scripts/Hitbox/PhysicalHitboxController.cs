﻿using Assets.Ancible_Tools.Scripts.System;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.Hitbox
{
    public class PhysicalHitboxController : HitboxController
    {
        void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.parent)
            {
                var enterCollisionMsg = MessageFactory.GenerateEnterCollisionWithObjectMsg();
                enterCollisionMsg.Object = collision.transform.parent.gameObject;
                for (var i = 0; i < _subscribers.Count; i++)
                {
                    gameObject.SendMessageTo(enterCollisionMsg, _subscribers[i]);
                }
                MessageFactory.CacheMessage(enterCollisionMsg);
            }
        }

        void OnCollisionExit(Collision collision)
        {
            if (collision.transform.parent)
            {
                var exitCollisionMsg = MessageFactory.GenerateExitCollisionWithObjectMsg();
                exitCollisionMsg.Object = collision.transform.parent.gameObject;
                for (var i = 0; i < _subscribers.Count; i++)
                {
                    gameObject.SendMessageTo(exitCollisionMsg, _subscribers[i]);
                }
                MessageFactory.CacheMessage(exitCollisionMsg);
            }
        }
    }
}