using Assets.Ancible_Tools.Scripts.Traits;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System.WorldSelect
{
    public class UnitSelectorController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _defaultCellSize = .22f;
        [SerializeField] private float _sizePerCell = .3f;

        public void Select(GameObject obj)
        {
            SpriteTrait sprite = null;
            var offset = Vector2.zero;
            var querySpriteMsg = MessageFactory.GenerateQuerySpriteMsg();
            querySpriteMsg.DoAfter = spriteTrait => sprite = spriteTrait;
            gameObject.SendMessageTo(querySpriteMsg, obj);
            MessageFactory.CacheMessage(querySpriteMsg);

            if (sprite)
            {
                offset = sprite.Offset;
            }

            transform.SetParent(obj.transform);
            transform.localPosition = offset;
            gameObject.SetActive(true);
        }

        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }

        public void Unselect(GameObject obj, Transform parent)
        {
            ResetSelector(parent);
        }

        public void ResetSelector(Transform parent)
        {
            transform.SetParent(parent);
            transform.localPosition = Vector2.zero;
            _spriteRenderer.color = Color.white;
            gameObject.SetActive(false);

        }

        public void SetColor(Color color)
        {
            _spriteRenderer.color = color;
        }

        public void SetArea(int area)
        {
            var size = _defaultCellSize;
            size = size + (_sizePerCell * area);
            _spriteRenderer.size = new Vector2(size, size);
        }
    }
}