using System;
using System.Linq;
using AncibleCoreCommon.CommonData.CharacterClasses;
using Assets.Resources.Ancible_Tools.Scripts.Server.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.System.CharacterClasses
{
    [Serializable]
    public class ClassLevelUp
    {
        [SerializeField] private ServerTrait[] _applyOnLevelUp = new ServerTrait[0];

        public ClassLevelUpData GetData()
        {
            return new ClassLevelUpData {ApplyOnLevel = _applyOnLevelUp.Where(t => t).Select(t => t.name).ToArray()};
        }
    }
}