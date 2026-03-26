using System;
using Dialogue;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI.Interfaces
{
    public class DialogueCanvas : MonoBehaviour
    {
        [Inject] private DialogueManager dialogueManager;
        [SerializeField] private TextMeshProUGUI text;
        private Canvas canvas;

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
        }

        public void Awake()
        {
            canvas = GetComponent<Canvas>();
        }

        public void Open()
        {
            canvas.gameObject.SetActive(false);
        }
        
        public void Close()
        {
            canvas.gameObject.SetActive(true);
        }

        // might be deprecated
        public void Initialize(TextAsset json, string knotName)
        {
            dialogueManager.EnterDialogue(json, knotName);
        }

        public void DisplayDialogue(DialogueLine line)
        {
            text.text = line.text;
        }
    }
}