using System.Collections.Generic;
using System.Linq;
using AncibleCoreCommon;
using Assets.Ancible_Tools.Scripts.Traits;
using Assets.Resources.Ancible_Tools.Scripts.UI;
using Assets.Resources.Ancible_Tools.Scripts.UI.Nameplate;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class ObjectManagerController : MonoBehaviour
    {
        public static GameObject PlayerObject { get; private set; }
        public static string PlayerObjectId => _instance._playerObjId;

        private static ObjectManagerController _instance = null;

        [SerializeField] private Trait[] _playerTraits = new Trait[0];
        [SerializeField] private Trait[] _objectTraits = new Trait[0];

        private Dictionary<string, GameObject> _allObjects = new Dictionary<string, GameObject>();
        private WorldState _worldState = WorldState.Disconnected;

        private SetPositionMessage _setPositionMsg = new SetPositionMessage();
        private NetworkObjectPositionUpdateMessage _networkObjectPositionUpdateMsg = new NetworkObjectPositionUpdateMessage();
        private SetNetworkObjectDataMessage _setNetworkObjectDataMsg = new SetNetworkObjectDataMessage();

        private string _playerObjId = string.Empty;

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

        public static GameObject GetWorldObjectById(string id)
        {
            if (_instance._allObjects.TryGetValue(id, out var obj))
            {
                return obj;
            }

            return null;
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<ClientEnterWorldWithCharacterResultMessage>(ClientEnterWorldResult);
            gameObject.Subscribe<ClientObjectUpdateMessage>(ClientObjectUpdate);
            gameObject.Subscribe<ClientTransferToMapMessage>(ClientTransferToMap);
            gameObject.Subscribe<LeaveWorldMessage>(LeaveWorld);
        }

        private void ClientEnterWorldResult(ClientEnterWorldWithCharacterResultMessage msg)
        {
            if (msg.Success)
            {
                if (PlayerObject)
                {
                    Destroy(PlayerObject);
                }

                var pos = WorldController.GetWorldPositionFromTile(msg.Data.Position);

                PlayerObject = Instantiate(FactoryController.UNIT_CONTROLLER, Vector3.zero, Quaternion.identity).gameObject;



                var addTraitToUnitMsg = MessageFactory.GenerateAddTraitToUnitMsg();
                var spriteTrait = TraitFactoryController.GetSpriteTraitByName(msg.Data.Sprite);
                if (spriteTrait)
                {
                    addTraitToUnitMsg.Trait = spriteTrait;
                    this.SendMessageTo(addTraitToUnitMsg, PlayerObject);
                }
                for (var i = 0; i < _playerTraits.Length; i++)
                {
                    addTraitToUnitMsg.Trait = _playerTraits[i];
                    gameObject.SendMessageTo(addTraitToUnitMsg, PlayerObject);
                }

                _setPositionMsg.Position = pos;
                _setPositionMsg.Tile = msg.Data.Position.ToVector();
                gameObject.SendMessageTo(_setPositionMsg, PlayerObject);
                _playerObjId = msg.ObjectId;
            }
        }

        private void ClientObjectUpdate(ClientObjectUpdateMessage msg)
        {
            if (DataController.WorldState == WorldState.Active)
            {
                var objData = msg.Objects.ToArray();
                for (var i = 0; i < objData.Length; i++)
                {
                    if (objData[i].ObjectId != _playerObjId)
                    {
                        var newObj = false;
                        var nameplateOffset = Vector2.zero;
                        if (!_allObjects.TryGetValue(objData[i].ObjectId, out var existingObj))
                        {
                            newObj = true;
                            var pos = WorldController.GetWorldPositionFromTile(objData[i].Position);
                            existingObj = Instantiate(FactoryController.UNIT_CONTROLLER, pos, Quaternion.identity).gameObject;
                            existingObj.name = objData[i].ObjectId;
                            var addTraitToUnitMsg = MessageFactory.GenerateAddTraitToUnitMsg();


                            if (!string.IsNullOrEmpty(objData[i].Sprite))
                            {
                                var sprite = TraitFactoryController.GetSpriteTraitByName(objData[i].Sprite);
                                if (sprite)
                                {
                                    nameplateOffset = sprite.NameplateOffset;
                                    addTraitToUnitMsg.Trait = sprite;
                                    gameObject.SendMessageTo(addTraitToUnitMsg, existingObj);
                                }
                            }
                            for (var t = 0; t < _objectTraits.Length; t++)
                            {
                                addTraitToUnitMsg.Trait = _objectTraits[t];
                                gameObject.SendMessageTo(addTraitToUnitMsg, existingObj);
                            }

                            var setPoistionMsg = MessageFactory.GenerateSetPositionMsg();
                            setPoistionMsg.Position = pos;
                            gameObject.SendMessageTo(setPoistionMsg, existingObj);
                            MessageFactory.CacheMessage(setPoistionMsg);
                            _allObjects.Add(objData[i].ObjectId, existingObj);

                        }

                        var forceUpdate = false;
                        if (!existingObj.gameObject.activeSelf)
                        {
                            forceUpdate = true;
                            existingObj.gameObject.SetActive(true);
                            gameObject.SendMessageTo(EnableObjectMessage.INSTANCE, existingObj);
                        }

                        _networkObjectPositionUpdateMsg.Tile = objData[i].Position.ToVector();
                        _networkObjectPositionUpdateMsg.Force = forceUpdate;
                        gameObject.SendMessageTo(_networkObjectPositionUpdateMsg, existingObj);

                        _setNetworkObjectDataMsg.Data = objData[i];
                        gameObject.SendMessageTo(_setNetworkObjectDataMsg, existingObj);

                        if (newObj && objData[i].ShowName)
                        {
                            UiNameplateManager.RegisterNameplate(existingObj, nameplateOffset);
                        }
                    }
                    //else
                    //{
                    //    _networkObjectPositionUpdateMsg.Tile = objData[i].Position.ToVector();
                    //    gameObject.SendMessageTo(_networkObjectPositionUpdateMsg, PlayerObject);
                    //}
                    //TODO: Update position;
                }

                var inactiveObjs = _allObjects.Keys.Where(k => msg.Objects.FirstOrDefault(o => o.ObjectId == k && o.ObjectId != _playerObjId) == null).ToArray();
                for (var i = 0; i < inactiveObjs.Length; i++)
                {
                    if (_allObjects.TryGetValue(inactiveObjs[i], out var inactive))
                    {
                        if (inactive.gameObject.activeSelf)
                        {
                            gameObject.SendMessageTo(DisableObjectMessage.INSTANCE, inactive);
                            inactive.gameObject.SetActive(false);
                        }
                    }
                }

                gameObject.SendMessage(RefreshWorldDataMessage.INSTANCE);
            }

        }

        private void ClientTransferToMap(ClientTransferToMapMessage msg)
        {
            var objs = _allObjects.Values.ToArray();
            for (var i = 0; i < objs.Length; i++)
            {
                gameObject.SendMessageTo(DisableObjectMessage.INSTANCE, objs[i]);
                objs[i].gameObject.SetActive(false);
            }
        }

        private void LeaveWorld(LeaveWorldMessage msg)
        {
            UiNameplateManager.ClearNameplates();
            var objs = _allObjects.Values.ToArray();
            for (var i = 0; i < objs.Length; i++)
            {
                Destroy(objs[i].gameObject);
            }
            _allObjects.Clear();
            Destroy(PlayerObject);
            PlayerObject = null;
            _playerObjId = string.Empty;
        }
    }
}