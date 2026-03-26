using System;
using Dialogue;
using UnityEngine;

namespace UI.Interfaces
{
    public class DialogueCanvas : MonoBehaviour
    {
        private Canvas canvas;

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

        public void Initialize(DialogueSO dialogue)
        {
            
        }
    }
}