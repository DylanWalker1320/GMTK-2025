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
   
    // You'd normally load this from a json / txt file using Ink for dialogue trees, but we only need a few lines so...
    private List<string> newGame1 = new List<string>
    {
        "... Where am I?",
        "...",
        "My hat! I must find my hat.",
    };
    private List<string> newGame2 = new List<string>
    {
        "This isn't my hat...",
        "It is now though!",
        "... Now where is my real hat?",
    };
    private List<string> newLoop1 = new List<string>
    {
        "... Here again!?",
        "...",
        "My hats!",
    };
    private List<string> newLoop2 = new List<string>
    {
        "I still need to find my hat...",
    };

    private bool isTyping = false;
    private int index = 0;
    private List<string> dialogueLines = new List<string>();

    public enum DialogueType
    {
        NewGame1,
        NewGame2,
        NewLoop1,
        NewLoop2
    }

    void Update()
    {
        if (isTyping)
        {
            dialogueBoxImage.enabled = true;
        }
        else
        {
            dialogueBoxImage.enabled = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!isTyping)
            {
                Debug.Log("Proceeding to next line.");
                NextLine();
            }
            else
            {
                Debug.Log("Skipping typing animation.");
                StopAllCoroutines();
                dialogueText.text = dialogueLines[index];
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            StartDialogue(DialogueType.NewGame1);
        }
    }

    List<string> PickDialogue(DialogueType type)
    {
        return type switch
        {
            DialogueType.NewGame1 => newGame1,
            DialogueType.NewGame2 => newGame2,
            DialogueType.NewLoop1 => newLoop1,
            DialogueType.NewLoop2 => newLoop2,
        };
    }

    void StartDialogue(DialogueType type)
    {
        Debug.Log("Starting conversation.");
        isTyping = true;
        dialogueText.text = string.Empty;
        dialogueLines = PickDialogue(type);

        StartCoroutine(TypeDialogue(dialogueLines[index]));
    }

    IEnumerator TypeDialogue(string line)
    {
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < dialogueLines.Count)
        {
            index++;
            dialogueText.text = string.Empty;
            StartCoroutine(TypeDialogue(dialogueLines[index]));
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        Debug.Log("End of conversation.");
        isTyping = false;
        index = 0;
        dialogueText.text = string.Empty;
        gameObject.SetActive(false);
    }
}