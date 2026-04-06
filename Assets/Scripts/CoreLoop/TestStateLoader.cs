using CoreLoop.Interfaces;
using CoreLoop.States;
using Dialogue;
using UnityEngine;
using Zenject;

namespace CoreLoop
{
    public class TestStateLoader : MonoBehaviour
    {
        public enum TestState
        {
            None = 0,
            MainMenu,
            Cinematic,
            Dialogue,
            Combat
        }
        [Inject] private readonly IGameStateMachine gameStateMachine;
        [Inject] private readonly MainMenuState.Factory mainMenuFactory;
        [Inject] private readonly CinematicState.Factory cinematicFactory;
        [Inject] private readonly DialogueState.Factory dialogueFactory;
        [Inject] private readonly CombatState.Factory combatFactory;

        [SerializeField] private TestState State;
        [SerializeField] private DialogueSO dialogueSo;
        
        private void Start()
        {
            LoadState();
        }
        
        private void LoadState()
        {
            switch (State)
            {
                case TestState.MainMenu:
                {
                    gameStateMachine.ChangeState(mainMenuFactory.Create());
                    break;
                }
                case TestState.Cinematic:
                {
                    gameStateMachine.ChangeState(cinematicFactory.Create("CinematicScene"));
                    break;
                }
                case TestState.Dialogue:
                {
                    gameStateMachine.ChangeState(dialogueFactory.Create(dialogueSo));
                    break;
                }
                case TestState.Combat:
                {
                    gameStateMachine.ChangeState(combatFactory.Create());
                    break;
                }
            }
        }

    }
}