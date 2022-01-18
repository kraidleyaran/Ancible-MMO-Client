using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System.Projectiles
{
    public class ProjectileFactoryController : MonoBehaviour
    {
        private static ProjectileFactoryController _instance = null;

        [SerializeField] private string _projectilePath = string.Empty;
        [SerializeField] private ProjectileController _controller = null;

        private Dictionary<string, Projectile> _projectiles = new Dictionary<string, Projectile>();

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            _projectiles = UnityEngine.Resources.LoadAll<Projectile>(_projectilePath).ToDictionary(p => p.name, p => p);
        }

        public static Projectile GetProjectileByName(string projectileName)
        {
            if (_instance._projectiles.TryGetValue(projectileName, out var projectile))
            {
                return projectile;
            }

            return null;
        }

        public static ProjectileController GenerateController(Projectile projectile, Vector2 position, GameObject target, int time)
        {
            var controller = Instantiate(_instance._controller, position, Quaternion.identity);
            controller.Setup(projectile, time, target);
            return controller;
        }
    }
}