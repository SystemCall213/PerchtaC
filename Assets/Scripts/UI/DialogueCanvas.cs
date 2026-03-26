using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;

namespace UI.Interfaces
{
    public class DialogueCanvas : MonoBehaviour, IPointerClickHandler
    {
        [Inject] private DialogueManager dialogueManager;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private TextMeshProUGUI speaker;
        [SerializeField] private Image girlImage;
        [SerializeField] private Image perchtaImage;
        [SerializeField] private CanvasGroup canvasGroup;
        private InkDialogueVariables inkDialogueVariables;
        
        private Canvas canvas;

        private void OnEnable()
        {
            dialogueManager.OnDialogueEntered += Open;
            dialogueManager.OnDialogueExited += Close;
            dialogueManager.OnDialogueDisplay += DisplayDialogue;
            dialogueManager.OnUpdateInkDialogueVariable += UpdateDialogueInkVariable;
        }

        private void OnDisable()
        {
            dialogueManager.OnDialogueEntered -= Open;
            dialogueManager.OnDialogueExited -= Close;
            dialogueManager.OnDialogueDisplay -= DisplayDialogue;
            dialogueManager.OnUpdateInkDialogueVariable -= UpdateDialogueInkVariable;
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

        // might be deprecated
        public void Initialize(TextAsset json, string knotName)
        {
            dialogueManager.EnterDialogue(json, knotName);
        }

        public void DisplayDialogue(DialogueLine line)
        {
            text.text = line.text;
            speaker.text = line.speaker;
            girlImage.sprite = line.girlSprite;
            perchtaImage.sprite = line.perchtaSprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            dialogueManager.ContinueOrExitStory();
        }

        public void UpdateDialogueInkVariable(string name, Ink.Runtime.Object value)
        {
            inkDialogueVariables.UpdateVariableState(name, value);
        }
    }
}