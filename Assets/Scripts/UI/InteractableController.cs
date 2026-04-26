using System.Collections.Generic;
using CoreLoop.Interfaces;
using CoreLoop.States;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    [RequireComponent(typeof(Canvas))]
    public class InteractableController : MonoBehaviour
    {
        [Inject] private readonly IGameStateMachine gameStateMachine;
        [Inject] private readonly LoadGivenLevel.Factory loadGivenLevelFactory;
        [Inject] private readonly CombatState.Factory combatStateFactory;
        
        [SerializeField] private Button doorButton;
        [SerializeField] private List<Button> fightButtons;
        [SerializeField] private List<string> fightSceneNames;

        private void OnEnable()
        {
            doorButton.onClick.AddListener(GoToNextLevel);
            gameStateMachine.OnStateChanged += CheckButtonState;
            CheckButtonState(gameStateMachine.CurrentState);
            
            
            for (int i = 0; i < fightButtons.Count; i++)
            {
                int index = i;
                string sceneName = fightSceneNames.Count > index ? fightSceneNames[index] : "CombatScene";
                fightButtons[index].onClick.AddListener(() => Fight(sceneName));
            }
        }

        private void OnDisable()
        {
            doorButton.onClick.RemoveListener(GoToNextLevel);
            
            foreach (Button btn in fightButtons)
            {
                btn.onClick.RemoveAllListeners();
            }
            gameStateMachine.OnStateChanged -= CheckButtonState;
        }

        private void Fight(string sceneName)
        {
            gameStateMachine.ChangeState(combatStateFactory.Create(sceneName));
        }

        private void GoToNextLevel()
        {
            gameStateMachine.ChangeState(loadGivenLevelFactory.Create("TestScene"));
        }

        private void  CheckButtonState(State state)
        {
            if (state is RoomState || state is LoadGivenLevel || state is LoadNextLevel)
            {
                foreach (var fightButton in fightButtons)
                {
                    fightButton.interactable = true;
                    fightButton.gameObject.SetActive(true);
                }
            }

            else
            {
                foreach (var fightButton in fightButtons)
                {
                    fightButton.interactable = false;
                    fightButton.gameObject.SetActive(false);
                }
            }
        }
    }
}
