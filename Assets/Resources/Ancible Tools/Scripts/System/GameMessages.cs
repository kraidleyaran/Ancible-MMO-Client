using System;
using AncibleCoreCommon.CommonData.Client;
using AncibleCoreCommon.CommonData.Items;
using Assets.Ancible_Tools.Scripts.System.Input;
using Assets.Ancible_Tools.Scripts.System.Maps;
using Assets.Resources.Ancible_Tools.Scripts.Server.Items;
using Assets.Resources.Ancible_Tools.Scripts.System.Player;
using Assets.Resources.Ancible_Tools.Scripts.UI;
using Assets.Resources.Ancible_Tools.Scripts.UI.AbilityManager;
using Assets.Resources.Ancible_Tools.Scripts.UI.ActionBar;
using Assets.Resources.Ancible_Tools.Scripts.UI.Alerts;
using Assets.Resources.Ancible_Tools.Scripts.UI.Character;
using Assets.Resources.Ancible_Tools.Scripts.UI.Character_List;
using Assets.Resources.Ancible_Tools.Scripts.UI.Create_Character;
using Assets.Resources.Ancible_Tools.Scripts.UI.Inventory;
using Assets.Resources.Ancible_Tools.Scripts.UI.Loot;
using Assets.Resources.Ancible_Tools.Scripts.UI.Nameplate;
using Assets.Resources.Ancible_Tools.Scripts.UI.Shop;
using Assets.Resources.Ancible_Tools.Scripts.UI.Windows;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class LoginMessage : EventMessage
    {
        public string Username;
        public string Password;
    }

    public class ShowLoginMessage : EventMessage
    {
        public static ShowLoginMessage INSTANCE = new ShowLoginMessage();
    }

    public class HideLoginMessage : EventMessage
    {
        public static HideLoginMessage INSTANCE = new HideLoginMessage();
    }

    public class RefreshPlayerCharacterListMessage : EventMessage
    {
        public static RefreshPlayerCharacterListMessage INSTANCE = new RefreshPlayerCharacterListMessage();
    }

    public class SetSelectedPlayerCharacterInfoMessage : EventMessage
    {
        public UiCharacterInfoController Controller;
    }

    public class ShowPlayerCharacterListMessage : EventMessage
    {
        public static ShowPlayerCharacterListMessage INSTANCE = new ShowPlayerCharacterListMessage();
    }

    public class HidePlayerCharacterListMessage : EventMessage
    {
        public static HidePlayerCharacterListMessage INSTANCE = new HidePlayerCharacterListMessage();
    }

    public class SetSelectedCharacterClassMessage : EventMessage
    {
        public UiCharacterClassController Controller;
    }

    public class ShowCharacterCreateMessage : EventMessage
    {
        public static ShowCharacterCreateMessage INSTANCE = new ShowCharacterCreateMessage();
    }

    public class CloseCharacterCreateMessage : EventMessage
    {
        public static CloseCharacterCreateMessage INSTANCE = new CloseCharacterCreateMessage();
    }

    public class SetCameraPositionMessage : EventMessage
    {
        public Vector2 Position;
    }

    public class UpdateTickMessage : EventMessage
    {
        public static UpdateTickMessage INSTANCE = new UpdateTickMessage();
    }

    public class FixedUpdateTickMessage : EventMessage
    {
        public static FixedUpdateTickMessage INSTANCE = new FixedUpdateTickMessage();
    }

    public class WorldTickMessage : EventMessage
    {
        public static WorldTickMessage INSTANCE = new WorldTickMessage();
    }

    public class SetDirectionMessage : EventMessage
    {
        public Vector2Int Direction;
        public bool Clear;
    }

    public class UpdateDirectionMessage : EventMessage
    {
        public Vector2Int Direction;
    }

    public class QueryDirectionMessage : EventMessage
    {
        public Action<Vector2> DoAfter;
    }

    public class UpdateInputStateMessage : EventMessage
    {
        public WorldInputState Previous;
        public WorldInputState Current;
    }

    public class SetPositionMessage : EventMessage
    {
        public Vector2 Position;
        public Vector2Int Tile;
    }

    public class UpdatePositionMessage : EventMessage
    {
        public Vector2 Position;
    }

    public class UpdateTileMessage : EventMessage
    {
        public Vector2Int Tile;
    }

    public class NetworkObjectPositionUpdateMessage : EventMessage
    {
        public Vector2Int Tile;
        public bool Force;
    }

    public class DisableObjectMessage : EventMessage
    {
        public static DisableObjectMessage INSTANCE = new DisableObjectMessage();
    }

    public class EnableObjectMessage : EventMessage
    {
        public static EnableObjectMessage INSTANCE = new EnableObjectMessage();
    }

    public class SelectObjectMessage : EventMessage
    {
        public static SelectObjectMessage INSTANCE = new SelectObjectMessage();
    }

    public class UnselectObjectMessage : EventMessage
    {
        public static UnselectObjectMessage INSTANCE = new UnselectObjectMessage();
    }

    public class HoverObjectMessage : EventMessage
    {
        public static HoverObjectMessage INSTANCE = new HoverObjectMessage();
    }

    public class UnhoverObjectMessage : EventMessage
    {
        public static UnhoverObjectMessage INSTANCE = new UnhoverObjectMessage();
    }

    public class SetNetworkObjectDataMessage : EventMessage
    {
        public ClientObjectData Data;
    }

    public class QueryNetworkObjectDataMessage : EventMessage
    {
        public Action<ClientObjectData> DoAfter;
    }

    public class DoBumpMessage : EventMessage
    {
        public Vector2 Direction;
    }

    public class RefreshPlayerDataMessage : EventMessage
    {
        public static RefreshPlayerDataMessage INSTANCE = new RefreshPlayerDataMessage();
    }

    public class QueryTilePositionMessage : EventMessage
    {
        public Action<Vector2Int> DoAfter;
    }

    public class SetMovementPathMessage : EventMessage
    {
        public MapTile[] Path;
        public Action OnCompletedPath;
    }

    public class UpdateNextTileMessage : EventMessage
    {
        public Vector2Int Tile;
    }

    public class ReturnSelectedSelectorMessage : EventMessage
    {
        public static ReturnSelectedSelectorMessage INSTANCE = new ReturnSelectedSelectorMessage();
    }

    public class ReturnHoveredSelectorMessage : EventMessage
    {
        public static ReturnHoveredSelectorMessage INSTANCE = new ReturnHoveredSelectorMessage();
    }

    public class RemoveAlertMessage : EventMessage
    {
        public UiAlertController Controller;
    }

    public class RefreshGlobalCooldownMessage : EventMessage
    {
        public static RefreshGlobalCooldownMessage INSTANCE = new RefreshGlobalCooldownMessage();
    }

    public class UpdateNetworkObjectDataMessage : EventMessage
    {
        public ClientObjectData Data;
    }

    public class SetHoveredWindowMessage : EventMessage
    {
        public UiBaseWindow Window;
    }

    public class RemoveHoveredWindowMessage : EventMessage
    {
        public UiBaseWindow Window;
    }

    public class SetHoveredInventoryItemMessage : EventMessage
    {
        public UiInventoryItemController Controller;
    }

    public class RemoveHoveredInventoryItemMessage : EventMessage
    {
        public UiInventoryItemController Controller;
    }

    public class UpdateWorldStateMessage : EventMessage
    {
        public WorldState State;
    }

    public class SetHoveredEquippedItemMessage : EventMessage
    {
        public UiEquippedItemController Controller;
    }

    public class RemoveHoveredEquippedItemMessage : EventMessage
    {
        public UiEquippedItemController Controller;
    }

    public class SetGeneralHoverInfoMessage : EventMessage
    {
        public string Title;
        public string Description;
        public Sprite Icon;
        public Color IconColor;
        public GameObject Owner;
    }

    public class SetItemHoverInfoMessage : EventMessage
    {
        public Item Item;
        public int Stack;
        public GameObject Owner;
    }

    public class SetShopItemHoverInfoMessage : EventMessage
    {
        public Item ShopItem;
        public int Cost;
        public int Stack;
        public GameObject Owner;
    }

    public class RemoveHoverInfoMessage : EventMessage
    {
        public GameObject Owner;
    }

    public class SetCharacterWindowStateMessage : EventMessage
    {
        public CharacterWindowState State;
    }

    public class SetHoveredShopItemMessage : EventMessage
    {
        public UiShopItemController Controller;
    }

    public class RemoveHoveredShopItemMessage : EventMessage
    {
        public UiShopItemController Controller;
    }

    public class ShowShopTransactionMessage : EventMessage
    {
        public ShopTransactionType Type;
        public string ObjectId;
        public string ItemId;
        public int Stack;
        public int Cost;
        public Item Item;
    }

    public class RefreshWorldDataMessage : EventMessage
    {
        public static RefreshWorldDataMessage INSTANCE = new RefreshWorldDataMessage();
    }

    public class SetHoveredAbilityMessage : EventMessage
    {
        public UiAbilityController Controller;
    }

    public class RemoveHoveredAbilityMessage : EventMessage
    {
        public UiAbilityController Controller;
    }

    public class SetHoveredActionBarItemMessage : EventMessage
    {
        public UiActionBarItemController Controller;
    }

    public class RemoveHoveredActionBarItemMessage : EventMessage
    {
        public UiActionBarItemController Controller;
    }

    public class CharacterSettingsLoadedMessage : EventMessage
    {
        public static CharacterSettingsLoadedMessage INSTANCE = new CharacterSettingsLoadedMessage();
    }

    public class RemoveStatusEffectMessage : EventMessage
    {
        public UiStatusEffectController Controller;
    }

    public class CancelCurrentAutoAbilityMessage : EventMessage
    {
        public static CancelCurrentAutoAbilityMessage INSTANCE = new CancelCurrentAutoAbilityMessage();
    }

    public class SetHoveredLootItemMessage : EventMessage
    {
        public UiLootItemController Controller;
    }

    public class RemoveHoveredLootItemMessage : EventMessage
    {
        public UiLootItemController Controller;
    }
    
}