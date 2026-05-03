using CoreLoop.Interfaces;
using UnityEngine;

namespace Interfaces
{
    [CreateAssetMenu(fileName = "TestDialogue", menuName = "Dialogue/DialogueSO")]
    public class DialogueSO : ScriptableObject, IStatePayload
    {
        public TextAsset json;
        public string knotName;
    }
}