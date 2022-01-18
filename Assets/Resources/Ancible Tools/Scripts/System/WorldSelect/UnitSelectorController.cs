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
            transform.SetParent(obj.transform);
            transform.localPosition = Vector2.zero;
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