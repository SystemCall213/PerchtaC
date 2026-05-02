using System;
using UnityEngine;

namespace Dialogue.Interfaces
{
    public interface IDialogueManager
    {
        public event Action OnDialogueEntered;
        public event Action OnDialogueExited;
        public event Action<DialogueLine> OnDialogueDisplay;

        void EnterDialogue(TextAsset dialogue, string knotName);
        void ContinueOrExitStory();
        void ChooseChoice(int choiceIndex);
    }
}