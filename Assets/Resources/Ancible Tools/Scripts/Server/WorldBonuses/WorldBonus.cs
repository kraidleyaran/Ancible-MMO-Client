using System;
using System.Linq;
using AncibleCoreCommon.CommonData.Traits;
using AncibleCoreCommon.CommonData.WorldBonuses;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.WorldBonuses
{
    [CreateAssetMenu(fileName = "World Bonus", menuName = "Ancible Tools/Server/World Bonuses/World Bonus")]
    public class WorldBonus : ScriptableObject
    {
        public WorldBonusType Type => _type;
        public WorldTag[] Tags => _tags;

        [SerializeField] private WorldBonusType _type;
        [SerializeField] private float _amount = 0f;
        [SerializeField] private ApplyValueType _applyType = ApplyValueType.Absolute;
        [SerializeField] private WorldTag[] _tags = new WorldTag[0];

        public WorldBonusData GetData()
        {
            return new WorldBonusData
            {
                Name = name,
                Amount = _amount,
                ApplyType = _applyType,
                Tags = _tags.Where(t => t).Select(t => t.name).ToArray(),
                Type = _type
            };
        }

        public string GetClientDescription()
        {
            var returnDescription = string.Empty;
            switch (_type)
            {
                case WorldBonusType.Damage:
                    returnDescription = $"+{(_applyType == ApplyValueType.Absolute ? $"{(int)_amount}" : $"{_amount:P} to base")} damage";
                    break;
                case WorldBonusType.Heal:
                    returnDescription = $"+{(_applyType == ApplyValueType.Absolute ? $"{(int)_amount}" : $"{_amount:P} to base")} heal";
                    break;
                case WorldBonusType.Chance:
                    returnDescription = $"+{(_applyType == ApplyValueType.Absolute ? $"{_amount:P} additional percent chance" : $"{_amount:P} to base percent chance")}";
                    break;
            }

            if (_tags.Length > 0)
            {
                returnDescription = $"{returnDescription} to";
                if (_tags.Length > 1)
                {
                    for (var i = 0; i < _tags.Length; i++)
                    {
                        if (i < _tags.Length)
                        {
                            returnDescription = $"{returnDescription} {_tags[i].DisplayName},";
                        }
                        else
                        {
                            returnDescription = $"{returnDescription} and {_tags[i].DisplayName}";
                        }
                    }
                }
                else
                {
                    returnDescription = $"{returnDescription} {_tags[0].DisplayName}";
                }

            }

            return returnDescription;
        }

        public bool HasTags(string[] tags)
        {
            for (var i = 0; i < tags.Length; i++)
            {
                if (_tags.FirstOrDefault(t => t.name == tags[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}