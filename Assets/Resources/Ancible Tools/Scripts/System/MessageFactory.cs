using System.Collections.Generic;
using Assets.Ancible_Tools.Scripts.Hitbox;
using Assets.Ancible_Tools.Scripts.Traits;
using Assets.Resources.Ancible_Tools.Scripts.UI;
using Assets.Resources.Ancible_Tools.Scripts.UI.Shop;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    public static class MessageFactory
    {
        private static List<AddTraitToUnitMessage> _addTraitToUnitCache = new List<AddTraitToUnitMessage>();
        private static List<RemoveTraitFromUnitMessage> _removeTraitFromUnitCache = new List<RemoveTraitFromUnitMessage>();
        private static List<RemoveTraitFromUnitByControllerMessage> _removeTraitFromUnitByControllerCache = new List<RemoveTraitFromUnitByControllerMessage>();
        private static List<TraitCheckMessage> _traitCheckCache = new List<TraitCheckMessage>();
        private static List<HitboxCheckMessage> _hitboxCheckCache = new List<HitboxCheckMessage>();
        private static List<EnterCollisionWithObjectMessage> _enterCollisiongWithObjectCache = new List<EnterCollisionWithObjectMessage>();
        private static List<ExitCollisionWithObjectMessage> _exitCollisionWithObjectCache = new List<ExitCollisionWithObjectMessage>();
        private static List<SetSelectedPlayerCharacterInfoMessage> _setSelectedPlayerCharacterInfoCache = new List<SetSelectedPlayerCharacterInfoMessage>();
        private static List<SetSelectedCharacterClassMessage> _setSelectedCharacterClassCache = new List<SetSelectedCharacterClassMessage>();
        private static List<UpdateDirectionMessage> _updateDirectionCache = new List<UpdateDirectionMessage>();
        private static List<QueryDirectionMessage> _queryDirectionCache = new List<QueryDirectionMessage>();
        private static List<SetPositionMessage> _setPositionCache = new List<SetPositionMessage>();
        private static List<UpdatePositionMessage> _updatePostionCache = new List<UpdatePositionMessage>();
        private static List<QueryTilePositionMessage> _queryTilePositionCache = new List<QueryTilePositionMessage>();
        private static List<RemoveAlertMessage> _removeAlertCache = new List<RemoveAlertMessage>();
        private static List<QueryNetworkObjectDataMessage> _queryNetworkObjectDataCache = new List<QueryNetworkObjectDataMessage>();
        private static List<UpdateNetworkObjectDataMessage> _updateNetworkObjectDataCache = new List<UpdateNetworkObjectDataMessage>();
        private static List<SetHoveredWindowMessage> _setHoveredWindowCache = new List<SetHoveredWindowMessage>();
        private static List<RemoveHoveredWindowMessage> _removeHoveredWindowCache = new List<RemoveHoveredWindowMessage>();
        private static List<SetHoveredInventoryItemMessage> _setHoveredInventoryItemCache = new List<SetHoveredInventoryItemMessage>();
        private static List<RemoveHoveredInventoryItemMessage> _removeHoveredInventoryItemCache = new List<RemoveHoveredInventoryItemMessage>();
        private static List<SetHoveredEquippedItemMessage> _setHoveredEquippedItemCache = new List<SetHoveredEquippedItemMessage>();
        private static List<RemoveHoveredEquippedItemMessage> _removeHoveredEquippedItemCache = new List<RemoveHoveredEquippedItemMessage>();
        private static List<SetGeneralHoverInfoMessage> _setGeneralHoverInfoCache = new List<SetGeneralHoverInfoMessage>();
        private static List<RemoveHoverInfoMessage> _removeHoverInfoCache = new List<RemoveHoverInfoMessage>();
        private static List<SetHoveredShopItemMessage> _setHoveredShopItemCache = new List<SetHoveredShopItemMessage>();
        private static List<RemoveHoveredShopItemMessage> _removeHoveredShopItemCache = new List<RemoveHoveredShopItemMessage>();
        private static List<SetItemHoverInfoMessage> _setItemHoverInfoCache = new List<SetItemHoverInfoMessage>();
        private static List<SetShopItemHoverInfoMessage> _setShopItemHoverInfoCache = new List<SetShopItemHoverInfoMessage>();
        private static List<ShowShopTransactionMessage> _showShopTransactionCache = new List<ShowShopTransactionMessage>();
        private static List<SetHoveredAbilityMessage> _setHoveredAbilityCache = new List<SetHoveredAbilityMessage>();
        private static List<RemoveHoveredAbilityMessage> _removeHoveredAbilityCache = new List<RemoveHoveredAbilityMessage>();
        private static List<SetHoveredActionBarItemMessage> _setHoveredActionBarItemCache = new List<SetHoveredActionBarItemMessage>();
        private static List<RemoveHoveredActionBarItemMessage> _removeHoveredActionBarItemCache = new List<RemoveHoveredActionBarItemMessage>();
        private static List<RemoveStatusEffectMessage> _removeStatusEffectCache = new List<RemoveStatusEffectMessage>();
        private static List<SetHoveredLootItemMessage> _setHoveredLootItemCache = new List<SetHoveredLootItemMessage>();
        private static List<RemoveHoveredLootItemMessage> _removeHoveredLootItemCache = new List<RemoveHoveredLootItemMessage>();
        private static List<ApplyTalentPointMessage> _applyTalentPointCache = new List<ApplyTalentPointMessage>();
        private static List<RemoveTalentPointMessage> _removeTalentPointCache = new List<RemoveTalentPointMessage>();

        public static AddTraitToUnitMessage GenerateAddTraitToUnitMsg()
        {
            if (_addTraitToUnitCache.Count > 0)
            {
                var message = _addTraitToUnitCache[0];
                _addTraitToUnitCache.Remove(message);
                return message;
            }

            return new AddTraitToUnitMessage();
        }

        public static RemoveTraitFromUnitMessage GenerateRemoveTraitFromUnitMsg()
        {
            if (_removeTraitFromUnitCache.Count > 0)
            {
                var message = _removeTraitFromUnitCache[0];
                _removeTraitFromUnitCache.Remove(message);
                return message;
            }

            return new RemoveTraitFromUnitMessage();
        }

        public static RemoveTraitFromUnitByControllerMessage GenerateRemoveTraitFromUnitByControllerMsg()
        {
            if (_removeTraitFromUnitByControllerCache.Count > 0)
            {
                var message = _removeTraitFromUnitByControllerCache[0];
                _removeTraitFromUnitByControllerCache.Remove(message);
                return message;
            }

            return new RemoveTraitFromUnitByControllerMessage();
        }

        public static HitboxCheckMessage GenerateHitboxCheckMsg()
        {
            if (_hitboxCheckCache.Count > 0)
            {
                var message = _hitboxCheckCache[0];
                _hitboxCheckCache.Remove(message);
                return message;
            }

            return new HitboxCheckMessage();
        }

        public static EnterCollisionWithObjectMessage GenerateEnterCollisionWithObjectMsg()
        {
            if (_enterCollisiongWithObjectCache.Count > 0)
            {
                var message = _enterCollisiongWithObjectCache[0];
                _enterCollisiongWithObjectCache.Remove(message);
                return message;
            }

            return new EnterCollisionWithObjectMessage();
        }

        public static ExitCollisionWithObjectMessage GenerateExitCollisionWithObjectMsg()
        {
            if (_exitCollisionWithObjectCache.Count > 0)
            {
                var message = _exitCollisionWithObjectCache[0];
                _exitCollisionWithObjectCache.Remove(message);
                return message;
            }

            return new ExitCollisionWithObjectMessage();
        }

        public static TraitCheckMessage GenerateTraitCheckMsg()
        {
            if (_traitCheckCache.Count > 0)
            {
                var message = _traitCheckCache[0];
                _traitCheckCache.Remove(message);
                return message;
            }

            return new TraitCheckMessage();
        }

        public static SetSelectedPlayerCharacterInfoMessage GenerateSetSelectedPlayerCharacterInfoMsg()
        {
            if (_setSelectedPlayerCharacterInfoCache.Count > 0)
            {
                var message = _setSelectedPlayerCharacterInfoCache[0];
                _setSelectedPlayerCharacterInfoCache.Remove(message);
                return message;
            }

            return new SetSelectedPlayerCharacterInfoMessage();
        }

        public static SetSelectedCharacterClassMessage GeneratedSetSelectedCharacterClassMsg()
        {
            if (_setSelectedCharacterClassCache.Count > 0)
            {
                var message = _setSelectedCharacterClassCache[0];
                _setSelectedCharacterClassCache.Remove(message);
                return message;
            }

            return new SetSelectedCharacterClassMessage();
        }

        public static UpdateDirectionMessage GenerateUpdateDirectionMsg()
        {
            if (_updateDirectionCache.Count > 0)
            {
                var message = _updateDirectionCache[0];
                _updateDirectionCache.Remove(message);
                return message;
            }

            return new UpdateDirectionMessage();
        }

        public static QueryDirectionMessage GenerateQueryDirectionMsg()
        {
            if (_queryDirectionCache.Count > 0)
            {
                var message = _queryDirectionCache[0];
                _queryDirectionCache.Remove(message);
                return message;
            }

            return new QueryDirectionMessage();
        }

        public static SetPositionMessage GenerateSetPositionMsg()
        {
            if (_setPositionCache.Count > 0)
            {
                var message = _setPositionCache[0];
                _setPositionCache.Remove(message);
                return message;
            }

            return new SetPositionMessage();
        }

        public static UpdatePositionMessage GenerateUpdatePositionMsg()
        {
            if (_updatePostionCache.Count > 0)
            {
                var message = _updatePostionCache[0];
                _updatePostionCache.Remove(message);
                return message;
            }

            return new UpdatePositionMessage();
        }

        public static QueryTilePositionMessage GenerateQueryTilePositionMsg()
        {
            if (_queryTilePositionCache.Count > 0)
            {
                var message = _queryTilePositionCache[0];
                _queryTilePositionCache.Remove(message);
                return message;
            }

            return new QueryTilePositionMessage();
        }

        public static RemoveAlertMessage GenerateRemoveAlertMsg()
        {
            if (_removeAlertCache.Count > 0)
            {
                var message = _removeAlertCache[0];
                _removeAlertCache.Remove(message);
                return message;
            }

            return new RemoveAlertMessage();
        }

        public static QueryNetworkObjectDataMessage GenerateQueryNetworkObjectDataMsg()
        {
            if (_queryNetworkObjectDataCache.Count > 0)
            {
                var message = _queryNetworkObjectDataCache[0];
                _queryNetworkObjectDataCache.Remove(message);
                return message;
            }

            return new QueryNetworkObjectDataMessage();
        }

        public static UpdateNetworkObjectDataMessage GenerateUpdateNetworkObjectDataMsg()
        {
            if (_updateNetworkObjectDataCache.Count > 0)
            {
                var message = _updateNetworkObjectDataCache[0];
                _updateNetworkObjectDataCache.Remove(message);
                return message;
            }

            return new UpdateNetworkObjectDataMessage();
        }

        public static SetHoveredWindowMessage GenerateSetHoveredWindowMsg()
        {
            if (_setHoveredWindowCache.Count > 0)
            {
                var message = _setHoveredWindowCache[0];
                _setHoveredWindowCache.Remove(message);
                return message;
            }

            return new SetHoveredWindowMessage();
        }

        public static RemoveHoveredWindowMessage GenerateRemoveHoveredWindowMsg()
        {
            if (_removeHoveredWindowCache.Count > 0)
            {
                var message = _removeHoveredWindowCache[0];
                _removeHoveredWindowCache.Remove(message);
                return message;
            }

            return new RemoveHoveredWindowMessage();
        }

        public static SetHoveredInventoryItemMessage GenerateSetHoveredInventoryItemMsg()
        {
            if (_setHoveredInventoryItemCache.Count > 0)
            {
                var message = _setHoveredInventoryItemCache[0];
                _setHoveredInventoryItemCache.Remove(message);
                return message;
            }

            return new SetHoveredInventoryItemMessage();
        }

        public static RemoveHoveredInventoryItemMessage GenerateRemoveHoveredInventoryItemMsg()
        {
            if (_removeHoveredInventoryItemCache.Count > 0)
            {
                var message = _removeHoveredInventoryItemCache[0];
                _removeHoveredInventoryItemCache.Remove(message);
                return message;
            }

            return new RemoveHoveredInventoryItemMessage();
        }

        public static SetHoveredEquippedItemMessage GenerateSetHoveredEquippedItemMsg()
        {
            if (_setHoveredEquippedItemCache.Count > 0)
            {
                var message = _setHoveredEquippedItemCache[0];
                _setHoveredEquippedItemCache.Remove(message);
                return message;
            }

            return new SetHoveredEquippedItemMessage();
        }

        public static RemoveHoveredEquippedItemMessage GenerateRemoveHoveredEquippedItemMsg()
        {
            if (_removeHoveredEquippedItemCache.Count > 0)
            {
                var message = _removeHoveredEquippedItemCache[0];
                _removeHoveredEquippedItemCache.Remove(message);
                return message;
            }

            return new RemoveHoveredEquippedItemMessage();
        }

        public static SetGeneralHoverInfoMessage GenerateSetGeneralHoverInfoMsg()
        {
            if (_setGeneralHoverInfoCache.Count > 0)
            {
                var message = _setGeneralHoverInfoCache[0];
                _setGeneralHoverInfoCache.Remove(message);
                return message;
            }

            return new SetGeneralHoverInfoMessage{IconColor = Color.white};
        }

        public static RemoveHoverInfoMessage GenerateRemoveHoverInfoMsg()
        {
            if (_removeHoverInfoCache.Count > 0)
            {
                var message = _removeHoverInfoCache[0];
                _removeHoverInfoCache.Remove(message);
                return message;
            }

            return new RemoveHoverInfoMessage();
        }

        public static SetHoveredShopItemMessage GenerateSetHoveredShopItemMsg()
        {
            if (_removeHoveredShopItemCache.Count > 0)
            {
                var message = _setHoveredShopItemCache[0];
                _setHoveredShopItemCache.Remove(message);
                return message;
            }

            return new SetHoveredShopItemMessage();
        }

        public static RemoveHoveredShopItemMessage GenerateRemoveHoveredShopItemMsg()
        {
            if (_removeHoveredShopItemCache.Count > 0)
            {
                var message = _removeHoveredShopItemCache[0];
                _removeHoveredShopItemCache.Remove(message);
                return message;
            }

            return new RemoveHoveredShopItemMessage();
        }

        public static SetItemHoverInfoMessage GenerateSetItemHoverInfoMsg()
        {
            if (_setItemHoverInfoCache.Count > 0)
            {
                var message = _setItemHoverInfoCache[0];
                _setItemHoverInfoCache.Remove(message);
                return message;
            }

            return new SetItemHoverInfoMessage();
        }

        public static SetShopItemHoverInfoMessage GenerateSetShopItemHoverInfoMsg()
        {
            if (_setShopItemHoverInfoCache.Count > 0)
            {
                var message = _setShopItemHoverInfoCache[0];
                _setShopItemHoverInfoCache.Remove(message);
                return message;
            }

            return new SetShopItemHoverInfoMessage();
        }

        public static ShowShopTransactionMessage GenerateShowShopTransactionMsg()
        {
            if (_showShopTransactionCache.Count > 0)
            {
                var message = _showShopTransactionCache[0];
                _showShopTransactionCache.Remove(message);
                return message;
            }

            return new ShowShopTransactionMessage();
        }

        public static SetHoveredAbilityMessage GenerateSetHoveredAbilityMsg()
        {
            if (_setHoveredAbilityCache.Count > 0)
            {
                var message = _setHoveredAbilityCache[0];
                _setHoveredAbilityCache.Remove(message);
                return message;                
            }

            return new SetHoveredAbilityMessage();
        }

        public static RemoveHoveredAbilityMessage GenerateRemoveHoveredAbilityMsg()
        {
            if (_removeHoveredAbilityCache.Count > 0)
            {
                var message = _removeHoveredAbilityCache[0];
                _removeHoveredAbilityCache.Remove(message);
                return message;
            }

            return new RemoveHoveredAbilityMessage();
        }

        public static SetHoveredActionBarItemMessage GenerateSetHoveredActionBarItemMsg()
        {
            if (_setHoveredActionBarItemCache.Count > 0)
            {
                var message = _setHoveredActionBarItemCache[0];
                _setHoveredActionBarItemCache.Remove(message);
                return message;
            }

            return new SetHoveredActionBarItemMessage();
        }

        public static RemoveHoveredActionBarItemMessage GenerateRemoveHoveredActionBarItemMsg()
        {
            if (_removeHoveredActionBarItemCache.Count > 0)
            {
                var message = _removeHoveredActionBarItemCache[0];
                _removeHoveredActionBarItemCache.Remove(message);
                return message;
            }

            return new RemoveHoveredActionBarItemMessage();
        }

        public static RemoveStatusEffectMessage GenerateRemoveStatusEffectMessage()
        {
            if (_removeStatusEffectCache.Count > 0)
            {
                var message = _removeStatusEffectCache[0];
                _removeStatusEffectCache.Remove(message);
                return message;
            }

            return new RemoveStatusEffectMessage();
        }

        public static SetHoveredLootItemMessage GenerateSetHoveredLootItemMsg()
        {
            if (_setHoveredLootItemCache.Count > 0)
            {
                var message = _setHoveredLootItemCache[0];
                _setHoveredLootItemCache.Remove(message);
                return message;
            }

            return new SetHoveredLootItemMessage();
        }

        public static RemoveHoveredLootItemMessage GenerateRemoveHoveredLootItemMsg()
        {
            if (_removeHoveredLootItemCache.Count > 0)
            {
                var message = _removeHoveredLootItemCache[0];
                _removeHoveredLootItemCache.Remove(message);
                return message;
            }

            return new RemoveHoveredLootItemMessage();
        }

        public static ApplyTalentPointMessage GenerateApplyTalentPointMsg()
        {
            if (_applyTalentPointCache.Count > 0)
            {
                var message = _applyTalentPointCache[0];
                _applyTalentPointCache.Remove(message);
                return message;
            }

            return new ApplyTalentPointMessage();
        }

        public static RemoveTalentPointMessage GenerateRemoveTalentPointMsg()
        {
            if (_removeTalentPointCache.Count > 0)
            {
                var message = _removeTalentPointCache[0];
                _removeTalentPointCache.Remove(message);
                return message;
            }

            return new RemoveTalentPointMessage();
        }

        //TODO: Start Cache

        public static void CacheMessage(AddTraitToUnitMessage msg)
        {
            msg.Trait = null;
            msg.Sender = null;
            _addTraitToUnitCache.Add(msg);
        }

        public static void CacheMessage(RemoveTraitFromUnitMessage msg)
        {
            msg.Trait = null;
            msg.Sender = null;
            _removeTraitFromUnitCache.Add(msg);
        }

        public static void CacheMessage(RemoveTraitFromUnitByControllerMessage msg)
        {
            msg.Controller = null;
            msg.Sender = null;
            _removeTraitFromUnitByControllerCache.Add(msg);
        }

        public static void CacheMessage(HitboxCheckMessage msg)
        {
            msg.DoAfter = null;
            msg.Sender = null;
            _hitboxCheckCache.Add(msg);
        }

        public static void CacheMessage(EnterCollisionWithObjectMessage msg)
        {
            msg.Object = null;
            msg.Sender = null;
            _enterCollisiongWithObjectCache.Add(msg);
        }

        public static void CacheMessage(ExitCollisionWithObjectMessage msg)
        {
            msg.Object = null;
            msg.Sender = null;
            _exitCollisionWithObjectCache.Add(msg);
        }

        public static void CacheMessage(TraitCheckMessage msg)
        {
            msg.DoAfter = null;
            msg.TraitsToCheck = null;
            msg.Sender = null;
            _traitCheckCache.Add(msg);
        }

        public static void CacheMessage(SetSelectedPlayerCharacterInfoMessage msg)
        {
            msg.Controller = null;
            msg.Sender = null;
            _setSelectedPlayerCharacterInfoCache.Add(msg);
        }

        public static void CacheMessage(SetSelectedCharacterClassMessage msg)
        {
            msg.Controller = null;
            msg.Sender = null;
            _setSelectedCharacterClassCache.Add(msg);
        }

        public static void CacheMessage(UpdateDirectionMessage msg)
        {
            msg.Direction = Vector2Int.zero;
            msg.Sender = null;
            _updateDirectionCache.Add(msg);

        }

        public static void CacheMessage(QueryDirectionMessage msg)
        {
            msg.DoAfter = null;
            msg.Sender = null;
            _queryDirectionCache.Add(msg);
        }

        public static void CacheMessage(SetPositionMessage msg)
        {
            msg.Position = Vector2.zero;
            msg.Tile = Vector2Int.zero;
            msg.Sender = null;
           _setPositionCache.Add(msg);
        }

        public static void CacheMessage(UpdatePositionMessage msg)
        {
            msg.Position = Vector2.zero;
            msg.Sender = null;
            _updatePostionCache.Add(msg);
        }

        public static void CacheMessage(QueryTilePositionMessage msg)
        {
            msg.DoAfter = null;
            msg.Sender = null;
            _queryTilePositionCache.Add(msg);
        }

        public static void CacheMessage(RemoveAlertMessage msg)
        {
            msg.Controller = null;
            msg.Sender = null;
            _removeAlertCache.Add(msg);
        }

        public static void CacheMessage(QueryNetworkObjectDataMessage msg)
        {
            msg.DoAfter = null;
            msg.Sender = null;
            _queryNetworkObjectDataCache.Add(msg);
        }

        public static void CacheMessage(UpdateNetworkObjectDataMessage msg)
        {
            msg.Data = null;
            msg.Sender = null;
            _updateNetworkObjectDataCache.Add(msg);
        }

        public static void CacheMessage(SetHoveredWindowMessage msg)
        {
            msg.Window = null;
            msg.Sender = null;
            _setHoveredWindowCache.Add(msg);
        }

        public static void CacheMessage(RemoveHoveredWindowMessage msg)
        {
            msg.Window = null;
            msg.Sender = null;
            _removeHoveredWindowCache.Add(msg);
        }

        public static void CacheMessage(SetHoveredInventoryItemMessage msg)
        {
            msg.Controller = null;
            msg.Sender = null;
            _setHoveredInventoryItemCache.Add(msg);
        }

        public static void CacheMessage(RemoveHoveredInventoryItemMessage msg)
        {
            msg.Controller = null;
            msg.Sender = null;
            _removeHoveredInventoryItemCache.Add(msg);
        }

        public static void CacheMessage(SetHoveredEquippedItemMessage msg)
        {
            msg.Controller = null;
            msg.Sender = null;
            _setHoveredEquippedItemCache.Add(msg);
        }

        public static void CacheMessage(RemoveHoveredEquippedItemMessage msg)
        {
            msg.Controller = null;
            msg.Sender = null;
            _removeHoveredEquippedItemCache.Add(msg);
        }

        public static void CacheMessage(SetGeneralHoverInfoMessage msg)
        {
            msg.Icon = null;
            msg.Title = string.Empty;
            msg.Description = string.Empty;
            msg.Owner = null;
            msg.IconColor = Color.white;
            msg.Sender = null;
            msg.WorldPosition = false;
            _setGeneralHoverInfoCache.Add(msg);
        }

        public static void CacheMessage(RemoveHoverInfoMessage msg)
        {
            msg.Owner = null;
            msg.Sender = null;
            _removeHoverInfoCache.Add(msg);
        }

        public static void CacheMessage(SetHoveredShopItemMessage msg)
        {
            msg.Controller = null;
            msg.Sender = null;
            _setHoveredShopItemCache.Add(msg);
        }

        public static void CacheMessage(RemoveHoveredShopItemMessage msg)
        {
            msg.Controller = null;
            msg.Sender = null;
            _removeHoveredShopItemCache.Add(msg);
        }

        public static void CacheMessage(SetItemHoverInfoMessage msg)
        {
            msg.Item = null;
            msg.Owner = null;
            msg.Stack = 0;
            msg.Sender = null;
            _setItemHoverInfoCache.Add(msg);
        }

        public static void CacheMessage(SetShopItemHoverInfoMessage msg)
        {
            msg.ShopItem = null;
            msg.Stack = 0;
            msg.Cost = 0;
            msg.Owner = null;
            msg.Sender = null;
            _setShopItemHoverInfoCache.Add(msg);
        }

        public static void CacheMessage(ShowShopTransactionMessage msg)
        {
            msg.Item = null;
            msg.ItemId = string.Empty;
            msg.ObjectId = string.Empty;
            msg.Stack = 0;
            msg.Cost = 0;
            msg.Type = ShopTransactionType.Buy;
            msg.Sender = null;
            _showShopTransactionCache.Add(msg);
        }

        public static void CacheMessage(SetHoveredAbilityMessage msg)
        {
            msg.Controller = null;
            msg.Sender = null;
            _setHoveredAbilityCache.Add(msg);
        }

        public static void CacheMessage(RemoveHoveredAbilityMessage msg)
        {
            msg.Controller = null;
            msg.Sender = null;
            _removeHoveredAbilityCache.Add(msg);
        }

        public static void CacheMessage(SetHoveredActionBarItemMessage msg)
        {
            msg.Controller = null;
            msg.Sender = null;
            _setHoveredActionBarItemCache.Add(msg);
        }

        public static void CacheMessage(RemoveHoveredActionBarItemMessage msg)
        {
            msg.Controller = null;
            msg.Sender = null;
            _removeHoveredActionBarItemCache.Add(msg);
        }

        public static void CacheMessage(RemoveStatusEffectMessage msg)
        {
            msg.Controller = null;
            msg.Sender = null;
            _removeStatusEffectCache.Add(msg);
        }

        public static void CacheMessage(SetHoveredLootItemMessage msg)
        {
            msg.Controller = null;
            msg.Sender = null;
            _setHoveredLootItemCache.Add(msg);
        }

        public static void CacheMessage(RemoveHoveredLootItemMessage msg)
        {
            msg.Controller = null;
            msg.Sender = null;
            _removeHoveredLootItemCache.Add(msg);
        }

        public static void CacheMessage(ApplyTalentPointMessage msg)
        {
            msg.Talent = null;
            msg.Sender = null;
            _applyTalentPointCache.Add(msg);
        }

        public static void CacheMessage(RemoveTalentPointMessage msg)
        {
            msg.Talent = null;
            msg.Sender = null;
            _removeTalentPointCache.Add(msg);
        }
    }

}