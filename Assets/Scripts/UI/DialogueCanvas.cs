using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Dialogue.Interfaces;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace UI.Interfaces
{
    public class DialogueCanvas : MonoBehaviour, IPointerClickHandler
    {
        [Inject] private IDialogueManager dialogueManager;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private TextMeshProUGUI speaker;
        [SerializeField] private Image girlImage;
        [SerializeField] private Image perchtaImage;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float typingSpeed = 0.05f;
        [SerializeField] private GameObject[] choices;
        private TextMeshProUGUI[] choicesText;
        
        private Canvas canvas;
        private CancellationTokenSource typingCts;
        private bool isTyping;
        private bool isChoosing;
        private string currentFullText;

        private void OnEnable()
        {
            dialogueManager.OnDialogueEntered += Open;
            dialogueManager.OnDialogueExited += Close;
            dialogueManager.OnDialogueDisplay += DisplayDialogue;
        }

        private void OnDisable()
        {
            dialogueManager.OnDialogueEntered -= Open;
            dialogueManager.OnDialogueExited -= Close;
            dialogueManager.OnDialogueDisplay -= DisplayDialogue;
            
            CancelTyping();
        }

        private void CancelTyping()
        {
            if (typingCts != null)
            {
                typingCts.Cancel();
                typingCts.Dispose();
                typingCts = null;
            }
        }

        public void Awake()
        {
            canvas = GetComponent<Canvas>();
        }

        public void Open()
        {
            canvas.gameObject.SetActive(true);

            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            canvasGroup.DOFade(1f, 2f).OnComplete(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            });
            
            choicesText = new TextMeshProUGUI[choices.Length];
            for (int i = 0; i < choices.Length; i++)            {
                choicesText[i] = choices[i].GetComponentInChildren<TextMeshProUGUI>();
            }
        }
        
        public void Close()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            canvasGroup.DOFade(0f, 2f).OnComplete(() =>
            {
                canvas.gameObject.SetActive(false);
            });
        }

        public void DisplayDialogue(DialogueLine line)
        {
            CancelTyping();

            speaker.text = line.speaker;
            if (line.girlSprite != null) girlImage.sprite = line.girlSprite;
            if (line.perchtaSprite != null) perchtaImage.sprite = line.perchtaSprite;

            DisplayChoices(line.choices);
            
            currentFullText = line.text;
            typingCts = new CancellationTokenSource();
            TypeText(currentFullText, typingCts.Token).Forget();
        }
        
        private void DisplayChoices(List<Choice> _choices)
        {
            if (_choices.Count > 0)
            {
                isChoosing = true;
            }
            
            if (_choices.Count > choices.Length)
            {
                Debug.LogWarning("More choices in Ink story than expected. Only displaying the first " + _choices.Count);
            }

            int index = 0;
            foreach (Choice choice in _choices)
            {
                choices[index].gameObject.SetActive(true);
                choicesText[index].text = choice.text;
                index++;
            }
        
            for (int i = index; i < choices.Length; i++)
            {
                choices[i].gameObject.SetActive(false);
            }
        }

        private async UniTaskVoid TypeText(string textToType, CancellationToken ct)
        {
            text.text = textToType;
            text.maxVisibleCharacters = 0;
            isTyping = true;

            // Use ForceMeshUpdate to ensure characterCount is accurate if text was just set
            text.ForceMeshUpdate();
            int totalCharacters = text.textInfo.characterCount;
            
            for (int i = 0; i <= totalCharacters; i++)
            {
                if (ct.IsCancellationRequested) break;
                
                text.maxVisibleCharacters = i;
                await UniTask.Delay((int)(typingSpeed * 1000), cancellationToken: ct).SuppressCancellationThrow();
            }
            
            if (!ct.IsCancellationRequested)
            {
                text.maxVisibleCharacters = totalCharacters;
                isTyping = false;
                typingCts?.Dispose();
                typingCts = null;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isTyping || isChoosing)
            {
                CancelTyping();
                text.maxVisibleCharacters = text.textInfo.characterCount;
                isTyping = false;
            }
            else
            {
                dialogueManager.ContinueOrExitStory();
            }
        }

        public void MakeChoice(int choiceIndex)
        {
            isChoosing = false;
            dialogueManager.ChooseChoice(choiceIndex);
        }
    }
}