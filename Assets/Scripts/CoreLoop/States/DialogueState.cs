using CoreLoop.Interfaces;
using Dialogue;
using UI.Interfaces;
using Zenject;

namespace CoreLoop.States
{
    public class DialogueState : State<DialogueSO>
    {
        [Inject] private readonly DialogueManager dialogueManager;

        public DialogueState(DialogueSO dialogueSO)
        {
            Payload = dialogueSO;
        }

        public override void Enter()
        {
            dialogueManager.EnterDialogue(Payload.json, Payload.knotName);
        }

        public override void Exit()
        {
            
        }
    }
}