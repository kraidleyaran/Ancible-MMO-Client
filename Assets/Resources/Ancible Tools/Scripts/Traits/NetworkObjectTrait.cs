using AncibleCoreCommon.CommonData.Client;
using Assets.Ancible_Tools.Scripts.System;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.Traits
{
    [CreateAssetMenu(fileName = "Network Object Trait", menuName = "Ancible Tools/Traits/Network/Network Object")]
    public class NetworkObjectTrait : Trait
    {
        private ClientObjectData _data = null;

        public override void SetupController(TraitController controller)
        {
            base.SetupController(controller);
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            _controller.transform.parent.gameObject.SubscribeWithFilter<SetNetworkObjectDataMessage>(SetNetworkObjectData, _instanceId);
            _controller.transform.parent.gameObject.SubscribeWithFilter<QueryNetworkObjectDataMessage>(QueryNetworkObjectData, _instanceId);
        }

        private void SetNetworkObjectData(SetNetworkObjectDataMessage msg)
        {
            _data = msg.Data;
            var updateNetworkObjDataMsg = MessageFactory.GenerateUpdateNetworkObjectDataMsg();
            updateNetworkObjDataMsg.Data = _data;
            _controller.gameObject.SendMessageTo(updateNetworkObjDataMsg, _controller.transform.parent.gameObject);
            MessageFactory.CacheMessage(updateNetworkObjDataMsg);
        }

        private void QueryNetworkObjectData(QueryNetworkObjectDataMessage msg)
        {
            msg.DoAfter.Invoke(_data);
        }

        
    }
}