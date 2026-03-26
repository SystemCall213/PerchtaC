using CoreLoop.Interfaces;
using Dialogue;
using UI.Interfaces;
using Zenject;

namespace CoreLoop.States
{
    public class DialogueState : State<DialogueSO>
    {
        [Inject] private readonly DialogueCanvas dialogueCanvas;

        [Inject]
        public DialogueState(DialogueSO dialogueSO)
        {
            Payload = dialogueSO;
        }

        public override void Enter()
        {
            dialogueCanvas.Open();
        }

        public override void Exit()
        {
            
        }

        public class Factory : PlaceholderFactory<DialogueSO, DialogueState> { }
    }
}