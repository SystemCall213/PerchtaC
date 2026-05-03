using UnityEngine;

namespace Interfaces
{
    [CreateAssetMenu(fileName = "TestDialogue", menuName = "Dialogue/DialogueSO")]
    public class DialogueSO : ScriptableObject
    {
        public TextAsset json;
        public string knotName;
    }
}