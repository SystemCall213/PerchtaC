using CoreLoop.Interfaces;
using Dialogue.Interfaces;
using Interfaces;
using Zenject;

namespace CoreLoop.States
{
    public class DialogueState : State<DialogueSO>
    {
        private readonly IDialogueManager dialogueManager;
        private readonly DefaultActions defaultActions;

        [Inject]
        public DialogueState(DialogueSO dialogueSO, IDialogueManager dialogueManager, DefaultActions defaultActions)
        {
            Payload = dialogueSO;
            this.dialogueManager = dialogueManager;
            this.defaultActions = defaultActions;
        }

        public override void Enter()
        {
            defaultActions.UI.CloseMenu.Enable();
            dialogueManager.EnterDialogue(Payload.json, Payload.knotName);
        }

        public override void Exit()
        {
            
        }

        public class Factory : PlaceholderFactory<DialogueSO, DialogueState> { }
    }
}