using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
// Dialogue data structure for JSON
[System.Serializable]
public class DialogueData
{
    public string dialogueId;
    public List<string> lines;
}

[System.Serializable]
public class DialogueCollection
{
    public List<DialogueData> dialogues;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image dialogueBoxImage;
    public bool debugMode = false;
    
    private bool isTyping = false;
    private bool dialogueActive = false;
    private int currentLineIndex = 0;
    private List<string> activeDialogueLines;
    private Coroutine typingCoroutine;
    private Action onDialogueComplete;
    
    // Dictionary to store loaded dialogues
    private Dictionary<string, List<string>> dialogueDatabase = new Dictionary<string, List<string>>();

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadDialoguesFromJSON("dialogues"); // Load from Resources folder
        dialogueBoxImage.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (dialogueActive)
            {
                if (isTyping)
                {
                    // Skip typing animation
                    CompleteCurrentLine();
                }
                else
                {
                    // Move to next line
                    AdvanceDialogue();
                }
            }
        }

        // Debug input to start a dialogue
        if (Input.GetKeyDown(KeyCode.T) && debugMode)
        {
            // Choose a random dialogue from the database to start
            if (dialogueDatabase.Count > 0)
            {
                String randomID = new List<string>(dialogueDatabase.Keys)[UnityEngine.Random.Range(0, dialogueDatabase.Count)];
                Debug.Log($"Starting dialogue with ID: {randomID}");
                StartDialogue(randomID);
            }
        }
    }

    public void LoadDialoguesFromJSON(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);
        if (jsonFile != null)
        {
            DialogueCollection collection = JsonUtility.FromJson<DialogueCollection>(jsonFile.text);
            foreach (DialogueData dialogue in collection.dialogues)
            {
                dialogueDatabase[dialogue.dialogueId] = dialogue.lines;
            }
            
            if (debugMode) Debug.Log($"Loaded {collection.dialogues.Count} dialogues from JSON");
        }
        else
        {
            if (debugMode) Debug.LogError($"Could not find dialogue file: {fileName}");
        }
    }

    public void StartDialogue(string dialogueId, Action onComplete = null)
    {
        if (dialogueDatabase.ContainsKey(dialogueId))
        {
            StartDialogue(dialogueDatabase[dialogueId], onComplete);
        }
        else
        {
            Debug.LogError($"Dialogue ID '{dialogueId}' not found in database!");
        }
    }

    public void StartDialogue(List<string> dialogueLines, Action onComplete = null)
    {
        if (dialogueActive) return;

        dialogueBoxImage.enabled = true;
        dialogueActive = true;
        activeDialogueLines = dialogueLines;
        currentLineIndex = 0;
        onDialogueComplete = onComplete;
        
        DisplayLine(activeDialogueLines[currentLineIndex]);
    }

    void DisplayLine(string line)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeDialogue(line));
    }

    void CompleteCurrentLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        dialogueText.text = activeDialogueLines[currentLineIndex];
        isTyping = false;
    }

    void AdvanceDialogue()
    {
        currentLineIndex++;
        
        if (currentLineIndex < activeDialogueLines.Count)
        {
            DisplayLine(activeDialogueLines[currentLineIndex]);
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialogueBoxImage.enabled = false;
        dialogueActive = false;
        currentLineIndex = 0;
        dialogueText.text = "";
        
        onDialogueComplete?.Invoke();
    }

    IEnumerator TypeDialogue(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        
        isTyping = false;
    }

    public bool IsDialogueActive()
    {
        return dialogueActive;
    }
}