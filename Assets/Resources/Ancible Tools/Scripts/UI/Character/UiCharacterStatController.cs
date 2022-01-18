using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Character
{
    public class UiCharacterStatController : MonoBehaviour
    {
        [SerializeField] private Text _statText;

        public void Setup(int baseStat, int bonusStat)
        {
            var total = baseStat + bonusStat;
            _statText.text = $"{total:n0} ({baseStat:n0}{(bonusStat > 0 || bonusStat < 0 ? $"{(bonusStat > 0 ? "+" : string.Empty)}{bonusStat:n0}" : string.Empty)})";
        }
    }
}