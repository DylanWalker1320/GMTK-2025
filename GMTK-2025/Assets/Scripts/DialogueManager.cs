using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class DialogueManager : MonoBehaviour
{   
    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image dialogueBoxImage;
    [SerializeField] private bool debugMode = false;
    private bool dialogueMode = false;
    private int currentLineIndex = 0;
    private List<string> activeDialogueLines;
   
    // You'd normally load this from a json / txt file using Ink for dialogue trees, but we only need a few lines so...
    public static List<string> newGame1 = new List<string>
    {
        "... Where am I?",
        "...",
        "My hat! I must find my hat.",
    };
    public static List<string> newGame2 = new List<string>
    {
        "This isn't my hat...",
        "It is now though!",
        "... Now where is my real hat?",
    };
    public static List<string> newLoop1 = new List<string>
    {
        "... Here again!?",
        "...",
        "My hats!",
    };
    public static List<string> newLoop2 = new List<string>
    {
        "I still need to find my hat...",
    };

    void StartDialogue(List<string> dialogueLines)
    {
        dialogueBoxImage.enabled = true;
        dialogueMode = true;
        activeDialogueLines = dialogueLines;
        currentLineIndex = 0;
        StartCoroutine(TypeDialogue(activeDialogueLines[currentLineIndex]));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (dialogueMode)
            {   // Advance dialogue
                currentLineIndex++;
                if (currentLineIndex < activeDialogueLines.Count)
                {
                    StopAllCoroutines();
                    dialogueText.text = activeDialogueLines[currentLineIndex];
                    StartCoroutine(TypeDialogue(activeDialogueLines[currentLineIndex]));
                } // End of dialogue
                else
                {
                    dialogueBoxImage.enabled = false;
                    dialogueMode = false;
                    currentLineIndex = 0;
                    dialogueText.text = "";
                }
            }
        }

        if (debugMode && Input.GetKeyDown(KeyCode.P))
        {
            // For testing purposes only
            StartDialogue(newGame1);
        }
    }

    IEnumerator TypeDialogue(string line)
    {
        dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }
}