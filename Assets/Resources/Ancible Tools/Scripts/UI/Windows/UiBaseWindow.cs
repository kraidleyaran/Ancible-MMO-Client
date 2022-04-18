using Assets.Ancible_Tools.Scripts.System;
using MessageBusLib;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Windows
{
    public class UiBaseWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public virtual bool Movable => true;
        public virtual bool Blocking => false;
        public virtual bool IsChild => true;

        [Header("General Window Information")]
        public string Title;
        [TextArea(3, 10)] public string Description;

        public void MovePosition(Vector2 delta)
        {
            var pos = transform.position;
            pos.x += delta.x;
            pos.y += delta.y;
            transform.position = pos;
        }

        public void SetPosition(Vector2 position)
        {
            var pos = transform.position;
            pos.x = position.x;
            pos.y = position.y;
            transform.position = pos;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            var setHoveredWindowMsg = MessageFactory.GenerateSetHoveredWindowMsg();
            setHoveredWindowMsg.Window = this;
            gameObject.SendMessage(setHoveredWindowMsg);
            MessageFactory.CacheMessage(setHoveredWindowMsg);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            var removeHoveredWindowMsg = MessageFactory.GenerateRemoveHoveredWindowMsg();
            removeHoveredWindowMsg.Window = this;
            gameObject.SendMessage(removeHoveredWindowMsg);
            MessageFactory.CacheMessage(removeHoveredWindowMsg);
        }

        public virtual void Close()
        {
            UiWindowManager.ToggleWindow(this);
        }

        public void Destroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}