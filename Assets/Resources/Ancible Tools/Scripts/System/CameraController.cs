using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class CameraController : MonoBehaviour
    {
        public static Camera Camera => _instance._camera;

        private static CameraController _instance = null;

        [SerializeField] private Camera _camera;

        private Vector2 _position = Vector2.zero;

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            _position = transform.position.ToVector2();
            SubscribeToMessages();
        }

        void Update()
        {
            var pos = transform.position;
            pos.x = _position.x;
            pos.y = _position.y;
            transform.position = pos;
        }

        //void LateUpdate()
        //{
        //    var pos = transform.position;
        //    pos.x = _position.x;
        //    pos.y = _position.y;
        //    transform.position = pos;
        //}

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<SetCameraPositionMessage>(SetCameraPosition);
        }

        private void SetCameraPosition(SetCameraPositionMessage msg)
        {
            _position = msg.Position;
        }


        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }

    }
}