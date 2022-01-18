using Assets.Ancible_Tools.Scripts.System;
using MessageBusLib;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.HoverInfo
{
    public class UiHoverInfoManager : MonoBehaviour
    {
        private static UiHoverInfoManager _instance = null;

        [Header("Prefab References")]
        [SerializeField] private UiGeneralInfoController _generalInfoTemplate;

        private GameObject _currentHoverInfo;
        private GameObject _currentOwner;

        private Vector2 _cursorPosition = Vector2.zero;

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
            gameObject.Subscribe<SetGeneralHoverInfoMessage>(SetGeneralHoverInfo);
            gameObject.Subscribe<RemoveHoverInfoMessage>(RemoveHoverInfo);
            gameObject.Subscribe<UpdateInputStateMessage>(UpdateInputState);
            gameObject.Subscribe<SetItemHoverInfoMessage>(SetItemHoverInfo);
            gameObject.Subscribe<SetShopItemHoverInfoMessage>(SetShopItemHoverInfo);
        }

        private void SetGeneralHoverInfo(SetGeneralHoverInfoMessage msg)
        {
            if (!_currentHoverInfo || _currentOwner != msg.Owner)
            {
                if (_currentHoverInfo)
                {
                    Destroy(_currentHoverInfo);
                }

                _currentOwner = msg.Owner;
                var generalHoverInfo = Instantiate(_generalInfoTemplate, transform);
                generalHoverInfo.Setup(msg.Title, msg.Description, msg.Icon);
                generalHoverInfo.SetIconColor(msg.IconColor);
                generalHoverInfo.SetPivot(StaticMethods.GetMouseQuadrant(_cursorPosition));
                _currentHoverInfo = generalHoverInfo.gameObject;
                _currentHoverInfo.transform.SetTransformPosition(_cursorPosition);
                
            }
            
        }

        private void RemoveHoverInfo(RemoveHoverInfoMessage msg)
        {
            if (_currentOwner && _currentOwner == msg.Owner)
            {
                Destroy(_currentHoverInfo);
                _currentOwner = null;
                _currentHoverInfo = null;
            }
        }

        private void UpdateInputState(UpdateInputStateMessage msg)
        {
            _cursorPosition = msg.Current.MousePosition;
            if (_currentHoverInfo)
            {
                _currentHoverInfo.transform.SetTransformPosition(_cursorPosition);
            }
        }

        private void SetItemHoverInfo(SetItemHoverInfoMessage msg)
        {
            if (!_currentOwner || msg.Owner != _currentOwner)
            {
                
                if (_currentHoverInfo)
                {
                    Destroy(_currentHoverInfo);
                }
                var item = msg.Item;
                _currentOwner = msg.Owner;
                var hoverInfo = Instantiate(_generalInfoTemplate, transform);
                var title = item.DisplayName;
                if (msg.Stack > 1)
                {
                    title = $"{StaticMethods.ApplyColorToText($"{title}", ColorFactoryController.GetColorFromItemRairty(item.Rarity))} ({msg.Stack})";
                }
                hoverInfo.SetupWithCost(title, item.GetDescription(), item.SellValue * msg.Stack, item.Icon);
                hoverInfo.SetPivot(StaticMethods.GetMouseQuadrant(_cursorPosition));
                _currentHoverInfo = hoverInfo.gameObject;
                _currentHoverInfo.transform.SetTransformPosition(_cursorPosition);
            }

        }

        private void SetShopItemHoverInfo(SetShopItemHoverInfoMessage msg)
        {
            if (!_currentOwner || msg.Owner != _currentOwner)
            {

                if (_currentHoverInfo)
                {
                    Destroy(_currentHoverInfo);
                }
                var item = msg.ShopItem;
                _currentOwner = msg.Owner;
                var hoverInfo = Instantiate(_generalInfoTemplate, transform);
                var title = item.DisplayName;
                if (msg.Stack > 1)
                {
                    title = $"{StaticMethods.ApplyColorToText($"{title}", ColorFactoryController.GetColorFromItemRairty(item.Rarity))} ({msg.Stack})";
                }
                hoverInfo.SetupWithCost(title, item.GetDescription(), msg.Cost, item.Icon);
                hoverInfo.SetPivot(StaticMethods.GetMouseQuadrant(_cursorPosition));
                _currentHoverInfo = hoverInfo.gameObject;
                _currentHoverInfo.transform.SetTransformPosition(_cursorPosition);
            }
        }
    }
}