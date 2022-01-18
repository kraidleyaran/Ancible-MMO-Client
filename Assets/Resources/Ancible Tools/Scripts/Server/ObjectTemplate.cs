using System.Linq;
using AncibleCoreCommon.CommonData;
using Assets.Resources.Ancible_Tools.Scripts.Server.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server
{
    [CreateAssetMenu(fileName = "Object Template", menuName = "Ancible Tools/Server/Templates/Object Template")]
    public class ObjectTemplate : ScriptableObject
    {
        [SerializeField] protected internal string _name;
        [SerializeField] protected internal ServerTrait[] _traits;

        public virtual ObjectTemplateData GetData()
        {
            return new ObjectTemplateData
            {
                Name = name,
                ObjectName = _name,
                Traits = _traits.Where(t => t).Select(t => t.name).ToArray()
            };
        }

        public virtual ServerTrait[] GetTraits()
        {
            return _traits;
        }
        
    }
}