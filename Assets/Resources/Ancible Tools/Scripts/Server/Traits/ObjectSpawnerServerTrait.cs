using AncibleCoreCommon.CommonData.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Object Spawner Server Trait", menuName = "Ancible Tools/Server/Traits/Object Spawner")]
    public class ObjectSpawnerServerTrait : ServerTrait
    {
        [SerializeField] private ObjectTemplate _template;
        [SerializeField] private int _spawnDistance = 1;
        [SerializeField] private int _maxSpawns = 1;
        [SerializeField] private int _spawnCooldown = 1;

        public override TraitData GetData()
        {
            return new ObjectSpawnerTraitData
            {
                Name = name,
                MaxStack = 1,
                MaxSpawns = _maxSpawns,
                SpawnDistance = _spawnDistance,
                Template = _template.name,
                SpawnCooldown = _spawnCooldown
            };
        }
    }
}