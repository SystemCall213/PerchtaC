using System.Collections.Generic;
using Audio;
using CoreLoop.Interfaces;
using CoreLoop.StatePayload;
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
        [Inject] private readonly RoomState.Factory roomStateFactory;
        [Inject] private readonly ISceneLoader sceneLoader;
        [SerializeField] private Button doorButton;
        [SerializeField] private List<Button> fightButtons;
        [SerializeField] private List<string> fightSceneNames;
        [SerializeField] private Sprite cleanRoomImage;
        [SerializeField] private Image background;
        [SerializeField] private Animator bgAnimator;

        private void OnEnable()
        {
            if (doorButton)
                doorButton.onClick.AddListener(GoToNextLevel);
            sceneLoader.BattleEnded += CleanRoom;
            
            for (int i = 0; i < fightButtons.Count; i++)
            {
                int index = i;
                string sceneName = fightSceneNames.Count > index ? fightSceneNames[index] : "CombatScene";
                fightButtons[index].onClick.AddListener(() => Fight(sceneName));
            }
        }

        private void OnDisable()
        {
            if (doorButton)
                doorButton.onClick.RemoveListener(GoToNextLevel);
            
            foreach (Button btn in fightButtons)
            {
                btn.onClick.RemoveAllListeners();
            }
            sceneLoader.BattleEnded -= CleanRoom;
        }

        private void Fight(string sceneName)
        {
            sceneLoader.LoadCombatScene(sceneName);
        }

        private void GoToNextLevel()
        {
            sceneLoader.LoadNextLevel();
        }
        
        private void CleanRoom()
        {
            foreach (var fightButton in fightButtons)
            {
                fightButton.interactable = false;
                fightButton.gameObject.SetActive(false);
            }

            if (doorButton)
            {
                doorButton.interactable = true;
                doorButton.gameObject.SetActive(true);
            }
            
            bgAnimator.enabled = false;
            background.sprite = cleanRoomImage;
        }
    }
}
