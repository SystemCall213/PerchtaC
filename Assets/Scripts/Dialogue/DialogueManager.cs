using System;
using CoreLoop.Interfaces;
using CoreLoop.StatePayload;
using CoreLoop.States;
using Dialogue.Interfaces;
using UnityEngine;
using Ink.Runtime;
using Zenject;

public class DialogueManager : IDialogueManager
{
    [Inject] private IGameStateMachine gameStateMachine;
    [Inject] private RoomState.Factory roomStateFactory;
    [Inject] private DialogueState.Factory dialogueStateFactory;
    [Header("Ink Story")]
    private Story story;

    private InkDialogueVariables inkDialogueVariables;
    private DialogueLine nextLine;

    public event Action OnDialogueEntered;
    public event Action OnDialogueExited;
    public event Action<DialogueLine> OnDialogueDisplay;
    
    public void EnterDialogue(TextAsset dialogue, string knotName)
    {
        story = new Story(dialogue.text);
        inkDialogueVariables = new InkDialogueVariables(story);
        nextLine = new DialogueLine();
        OnDialogueEntered?.Invoke();
        // gameStateMachine.ChangeState(dialogueStateFactory.Create(null));
        
        if (knotName != "")
        {
            story.ChoosePathString(knotName);
        }
        else
        {
            Debug.LogWarning("No knotname passed you idiot.");
        }
        
        inkDialogueVariables.SyncVariablesAndStartListening(story);

        // Assuming we need to set girl and perchta sprites once
        try
        {

            string girlSpriteName = story.variablesState["girl"].ToString();
            nextLine.girlSprite = Resources.Load<Sprite>(girlSpriteName);
        }
        catch (Exception e)
        {
            Debug.LogWarning("No girl sprite found");
        }
        // Start dialogue
        ContinueOrExitStory();
    }

    public void ContinueOrExitStory()
    {
        if (story.canContinue)
        {
            string text = story.Continue();
            nextLine.text = text;
            foreach (string tag in story.currentTags)
            {
                if (tag.StartsWith("speaker:"))
                {
                    string speaker = tag.Substring("speaker:".Length);
                    nextLine.speaker = speaker;
                }
            }
            
            OnDialogueDisplay?.Invoke(nextLine);
        }
        else
        {
            ExitDialogue();
        }
    }

    private void ExitDialogue()
    {
        inkDialogueVariables.StopListening(story);
        story.ResetState();
        gameStateMachine.ChangeState(roomStateFactory.Create(new RoomStatePayload()));
        OnDialogueExited?.Invoke();
    }
}
