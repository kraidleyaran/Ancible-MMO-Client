using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Dialogue
{
    [CreateAssetMenu(fileName = "Dialogue Data", menuName = "Ancible Tools/Data/Dialogue")]
    public class DialogueData : ScriptableObject
    {
        public string Title = string.Empty;
        [TextArea(3, 10)] public string[] Dialogue = new string[0];
        public DialogueTree Tree = new DialogueTree();
    }
}