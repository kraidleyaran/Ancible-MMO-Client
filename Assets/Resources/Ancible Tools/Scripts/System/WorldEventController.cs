using System.Collections.Generic;
using System.Linq;
using AncibleCoreCommon;
using AncibleCoreCommon.CommonData.WorldEvent;
using Assets.Ancible_Tools.Scripts.System.Projectiles;
using Assets.Resources.Ancible_Tools.Scripts.System.VisualEffects;
using Assets.Resources.Ancible_Tools.Scripts.UI.World_Events;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class WorldEventController : MonoBehaviour
    {
        private static WorldEventController _instance = null;

        private DoBumpMessage _doBumpMsg = new DoBumpMessage();

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

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<ClientWorldEventUpdateMessage>(ClientWorldEventUpdate);
        }

        private void ClientWorldEventUpdate(ClientWorldEventUpdateMessage msg)
        {
            var events = msg.Events.ToArray();
            for (var i = 0; i < events.Length; i++)
            {
                var worldEvent = AncibleUtils.FromJson<WorldEvent>(events[i]);
                switch (worldEvent.Type)
                {
                    case WorldEventType.Default:
                        break;
                    case WorldEventType.Bump:
                        var bumpEvent = AncibleUtils.FromJson<BumpWorldEvent>(events[i]);
                        if (bumpEvent != null)
                        {
                            var origin = bumpEvent.OriginId == ObjectManagerController.PlayerObjectId ? ObjectManagerController.PlayerObject : ObjectManagerController.GetWorldObjectById(bumpEvent.OriginId);
                            var target = bumpEvent.TargetId == ObjectManagerController.PlayerObjectId ? ObjectManagerController.PlayerObject : ObjectManagerController.GetWorldObjectById(bumpEvent.TargetId);
                            if (origin && target)
                            {
                                var direction = (target.transform.position.ToVector2() - origin.transform.position.ToVector2()).normalized;
                                _doBumpMsg.Direction = direction;
                                gameObject.SendMessageTo(_doBumpMsg, origin);
                            }
                        }
                        break;
                    case WorldEventType.Projectile:
                        var projectileEvent = AncibleUtils.FromJson<ProjectileWorldEvent>(events[i]);
                        if (projectileEvent != null)
                        {
                            var origin = projectileEvent.OwnerId == ObjectManagerController.PlayerObjectId ? ObjectManagerController.PlayerObject : ObjectManagerController.GetWorldObjectById(projectileEvent.OwnerId);
                            var target = projectileEvent.TargetId == ObjectManagerController.PlayerObjectId ? ObjectManagerController.PlayerObject : ObjectManagerController.GetWorldObjectById(projectileEvent.TargetId);
                            if (origin && target)
                            {
                                var startPosition = origin.transform.position.ToVector2();
                                var projectile = ProjectileFactoryController.GetProjectileByName(projectileEvent.Projectile);
                                if (projectile)
                                {
                                    ProjectileFactoryController.GenerateController(projectile, startPosition, target, projectileEvent.TravelTime);
                                }
                            }
                        }
                        break;
                    case WorldEventType.Fx:
                        var visualFxEvent = AncibleUtils.FromJson<VisualFxWorldEvent>(events[i]);
                        if (visualFxEvent != null)
                        {
                            var visualFx = VisualFxFactoryController.GetVisualFxByName(visualFxEvent.VisualFx);
                            if (visualFx)
                            {
                                var owner = visualFxEvent.OwnerId == ObjectManagerController.PlayerObjectId ? ObjectManagerController.PlayerObject : ObjectManagerController.GetWorldObjectById(visualFxEvent.OwnerId);
                                var pos = owner ? owner.transform.position.ToVector2() : WorldController.GetWorldPositionFromTile(visualFxEvent.OverridePosition);
                                var controller = Instantiate(VisualFxFactoryController.Controller, pos, Quaternion.identity);
                                controller.Setup(visualFx, owner);
                            }
                        }
                        break;
                    case WorldEventType.LevelUp:
                        var levelUpEvent = AncibleUtils.FromJson<LevelUpWorldEvent>(events[i]);
                        if (levelUpEvent != null)
                        {
                            var owner = levelUpEvent.OwnerId == ObjectManagerController.PlayerObjectId ? ObjectManagerController.PlayerObject : ObjectManagerController.GetWorldObjectById(levelUpEvent.OwnerId);
                            if (owner)
                            {
                                var pos = owner.transform.position.ToVector2();
                                var controller = Instantiate(VisualFxFactoryController.Controller, pos, Quaternion.identity);
                                controller.Setup(VisualFxFactoryController.LevelUpFx, owner);
                            }
                        }
                        break;
                }
                UiWorldEventManager.ShowEvent(events[i]);
            }
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}