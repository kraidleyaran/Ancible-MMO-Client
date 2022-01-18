using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.System.VisualEffects
{
    [CreateAssetMenu(fileName = "Visual Fx", menuName = "Ancible Tools/World/Visual Fx")]
    public class VisualFx : ScriptableObject
    {
        public RuntimeAnimatorController RuntimeController;
        public Vector2 Scale = Vector2.one;
        public Vector2 Offset = Vector2.zero;
        public int SortingOrder = 0;
    }
}