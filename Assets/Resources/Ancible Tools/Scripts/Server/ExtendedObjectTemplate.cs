using System.Linq;
using AncibleCoreCommon.CommonData;
using Assets.Resources.Ancible_Tools.Scripts.Server.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server
{
    [CreateAssetMenu(fileName = "Extended Object Template", menuName = "Ancible Tools/Server/Templates/Extended Object Template")]
    public class ExtendedObjectTemplate : ObjectTemplate
    {
        [SerializeField] private ObjectTemplate _parentTemplate;

        public override ObjectTemplateData GetData()
        {
            var traits = _parentTemplate.GetTraits().ToList();
            traits.AddRange(_traits);
            return new ObjectTemplateData { Name = name, ObjectName = _name, Traits = traits.Where(t => t).Select(t => t.name).ToArray()};
        }

        public override ServerTrait[] GetTraits()
        {
            var traits = _parentTemplate.GetTraits().ToList();
            traits.AddRange(_traits);
            return traits.ToArray();
        }
    }
}