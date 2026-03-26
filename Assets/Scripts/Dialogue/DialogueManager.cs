using System;
using UnityEngine;
using Ink.Runtime;

public class DialogueManager
{
    [Header("Ink Story")]
    private Story story;
    private bool dialoguePlaying = false;

    public event Action OnDialogueEntered;
    public event Action OnDialogueExited;
    public event Action<DialogueLine> OnDialogueDisplay;
    
    public void EnterDialogue(TextAsset dialogue, string knotName)
    {
        if (dialoguePlaying) return;
        
        dialoguePlaying = true;
        story = new Story(dialogue.text);
        OnDialogueEntered?.Invoke();
        
        if (knotName != "")
        {
            story.ChoosePathString(knotName);
        }
        else
        {
            Debug.LogWarning("No knotname passed you idiot.");
        }
        
        // Start dialogue
        ContinueOrExitStory();
    }

    private void ContinueOrExitStory()
    {
        if (story.canContinue)
        {
            string text = story.Continue();
            DialogueLine nextLine = new DialogueLine();
            nextLine.text = text;
            OnDialogueDisplay?.Invoke(nextLine);
        }
        else
        {
            ExitDialogue();
        }
    }

    private void ExitDialogue()
    {
        dialoguePlaying = false;
        OnDialogueExited?.Invoke();
        
        story.ResetState();
    }
}
