using System.Linq;
using AncibleCoreCommon.CommonData.Maps;
using Assets.Resources.Ancible_Tools.Scripts.Server.Traits;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System.Maps
{
    public class ObjectSpawnController : MonoBehaviour
    {
        [SerializeField] private string _name;
        [SerializeField] private string _subtitle = string.Empty;
        [SerializeField] private ServerTrait[] _traits;
        [SerializeField] private bool _visible = true;
        [SerializeField] private bool _blocking = false;
        [SerializeField] private bool _showName = true;

        public ObjectSpawnData GetData(Vector2Int position)
        {
            return new ObjectSpawnData
            {
                Name = _name,
                Subtitle = _subtitle,
                Position = position.ToData(),
                Traits = _traits.Where(t => t).Select(t => t.name).ToArray(),
                Visible = _visible,
                Blocking = _blocking,
                ShowName = _showName
            };
        }
    }
}