using CoreLoop.Interfaces;
using Dialogue.Interfaces;
using Interfaces;
using Zenject;

namespace CoreLoop.States
{
    public class DialogueState : State<DialogueSO>
    {
        private readonly IDialogueManager dialogueManager;

        [Inject]
        public DialogueState(DialogueSO dialogueSO, IDialogueManager dialogueManager)
        {
            Payload = dialogueSO;
            this.dialogueManager = dialogueManager;
        }

        public override void Enter()
        {
            dialogueManager.EnterDialogue(Payload.json, Payload.knotName);
        }

        public override void Exit()
        {
            
        }

        public class Factory : PlaceholderFactory<DialogueSO, DialogueState> { }
    }
}