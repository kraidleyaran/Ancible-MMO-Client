﻿using System.Collections.Generic;
using System.Linq;
using Assets.Ancible_Tools.Scripts.Traits;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class UnitController : MonoBehaviour
    {
        public Trait[] StartingTraits;

        private List<TraitController> _currentTraits = new List<TraitController>();

        private TraitController _instantController = null;

        internal virtual void Awake()
        {
            SubscribeToMessages();
        }

        internal virtual void Start()
        {
            _instantController = Instantiate(FactoryController.TRAIT_CONTROLLER, transform);
            var addTraitToUnitMsg = MessageFactory.GenerateAddTraitToUnitMsg();
            for (var i = 0; i < StartingTraits.Length; i++)
            {
                addTraitToUnitMsg.Trait = StartingTraits[i];
                gameObject.SendMessageTo(addTraitToUnitMsg, gameObject);
            }
            MessageFactory.CacheMessage(addTraitToUnitMsg);
        }

        internal virtual void SubscribeToMessages()
        {
            gameObject.Subscribe<AddTraitToUnitMessage>(AddTraitToUnit);
            gameObject.Subscribe<RemoveTraitFromUnitMessage>(RemoveTraitFromUnit);
            gameObject.Subscribe<RemoveTraitFromUnitByControllerMessage>(RemoveTraitFromUnitByController);
            gameObject.Subscribe<TraitCheckMessage>(TraitCheck);
            gameObject.Subscribe<CacheUnitMessage>(CacheUnit);
        }

        internal virtual void AddTraitToUnit(AddTraitToUnitMessage msg)
        {
            var traitCount = _currentTraits.Count(c => c.Trait.name == msg.Trait.name);
            if (traitCount < msg.Trait.MaxStack)
            {
                var controller = Instantiate(FactoryController.TRAIT_CONTROLLER, transform);
                _currentTraits.Add(controller);
                controller.Setup(msg.Trait);
            }
        }

        internal virtual void RemoveTraitFromUnit(RemoveTraitFromUnitMessage msg)
        {
            var controller = _currentTraits.Find(t => t.Trait.name == msg.Trait.name);
            if (controller)
            {
                controller.Destroy();
                _currentTraits.Remove(controller);
                Destroy(controller.gameObject);
            }
        }

        internal virtual void RemoveTraitFromUnitByController(RemoveTraitFromUnitByControllerMessage msg)
        {
            if (_currentTraits.Contains(msg.Controller))
            {
                msg.Controller.Destroy();
                _currentTraits.Remove(msg.Controller);
                Destroy(msg.Controller.gameObject);
            }
        }

        internal virtual void TraitCheck(TraitCheckMessage msg)
        {
            for (var i = 0; i < msg.TraitsToCheck.Count; i++)
            {
                if (_currentTraits.Exists(c => c.Trait.name == msg.TraitsToCheck[i].name))
                {
                    msg.DoAfter.Invoke();
                    return;
                }
            }
        }

        private void CacheUnit(CacheUnitMessage msg)
        {
            for (var i = 0; i < _currentTraits.Count; i++)
            {
                _currentTraits[i].ResetController();
            }
            UnitCacheController.CacheUnit(gameObject.name, this);
        }

        internal virtual void OnDestroy()
        {
            for (var i = 0; i < _currentTraits.Count; i++)
            {
                _currentTraits[i].Destroy();
                Destroy(_currentTraits[i].gameObject);
            }
            _currentTraits.Clear();
            if (_instantController)
            {
                Destroy(_instantController.gameObject);
            }
            gameObject.UnsubscribeFromAllMessages();
            
        }
    }
}