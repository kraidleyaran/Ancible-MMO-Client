﻿using AncibleCoreCommon.CommonData.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Ai State Server Trait", menuName = "Ancible Tools/Server/Traits/Ai/Ai State")]
    public class AiStateServerTrait : ServerTrait
    {
        public override TraitData GetData()
        {
            return new AiStateTraitData {Name = name, MaxStack = 1};
        }
    }
}