using AncibleCoreCommon.CommonData.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    public class ServerTrait : ScriptableObject
    {
        public int ClientDescriptionOrder = 0;
        public Sprite Icon;
        [SerializeField] protected internal int _maxStack = 1;
        


        public virtual TraitData GetData()
        {
            return new TraitData {Name = name, MaxStack = 1};
        }

        public virtual string GetClientDescriptor()
        {
            return string.Empty;
        }
    }
}