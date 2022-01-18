using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System.Projectiles
{
    [CreateAssetMenu(fileName = "Projectile", menuName = "Ancible Tools/World/Projectile")]
    public class Projectile : ScriptableObject
    {
        public Sprite Sprite;
        public Vector2 Scale = Vector2.one;
        public float RotationOffset = 0f;
        public bool Rotate;
    }
}