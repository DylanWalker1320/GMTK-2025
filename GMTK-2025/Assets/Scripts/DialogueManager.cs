using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

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

// Individual dialogue instance that tracks its own state
public class DialogueInstance
{
    public TextMeshProUGUI dialogueText;
    public Image dialogueBoxImage;
    public bool isTyping = false;
    public bool isActive = false;
    public int currentLineIndex = 0;
    public List<string> lines;
    public Coroutine typingCoroutine;
    public Action onComplete;
    public Action onCancel; // Callback for when dialogue is force-stopped
    public float textSpeed;
    public bool debugMode = false;
    
    public DialogueInstance(TextMeshProUGUI text, Image box, float speed)
    {
        dialogueText = text;
        dialogueBoxImage = box;
        textSpeed = speed;
    }
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Default UI (Screen Space)")]
    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private TextMeshProUGUI defaultDialogueText;
    [SerializeField] private Image defaultDialogueBoxImage;
    public bool debugMode = false;
    
    private Dictionary<string, List<string>> dialogueDatabase = new Dictionary<string, List<string>>();
    
    // Track multiple active dialogue instances
    private List<DialogueInstance> activeInstances = new List<DialogueInstance>();

    void Awake()
    {
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
        LoadDialoguesFromJSON("dialogues");
        
        if (defaultDialogueBoxImage != null)
            defaultDialogueBoxImage.enabled = false;
    }

    void Update()
    {
        // Handle input for all active dialogues
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            // Process each active instance
            for (int i = activeInstances.Count - 1; i >= 0; i--)
            {
                DialogueInstance instance = activeInstances[i];
                
                if (instance.isTyping)
                {
                    CompleteCurrentLine(instance);
                }
                else
                {
                    AdvanceDialogue(instance);
                }
            }
        }

        // Debug trigger for testing dialogues
        if (Input.GetKeyDown(KeyCode.T) && debugMode)
        {
            List<string> keys = new List<string>(dialogueDatabase.Keys);
            if (keys.Count > 0)
            {
                string randomKey = keys[UnityEngine.Random.Range(0, keys.Count)];
                if (debugMode) Debug.Log($"Debug Triggering Dialogue: {randomKey}");
                StartDialogue(randomKey);
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

    // Main method - starts dialogue with specific UI targets
    public DialogueInstance StartDialogue(string dialogueId, TextMeshProUGUI textTarget, Image boxTarget, Action onComplete = null, Action onCancel = null)
    {
        if (dialogueDatabase.ContainsKey(dialogueId))
        {
            return StartDialogue(dialogueDatabase[dialogueId], textTarget, boxTarget, onComplete, onCancel);
        }
        else
        {
            if (debugMode) Debug.LogError($"Dialogue ID '{dialogueId}' not found in database!");
            return null;
        }
    }

    public DialogueInstance StartDialogue(List<string> dialogueLines, TextMeshProUGUI textTarget, Image boxTarget, Action onComplete = null, Action onCancel = null)
    {
        // Check if this UI is already being used
        DialogueInstance existingInstance = FindInstanceByUI(textTarget, boxTarget);
        if (existingInstance != null)
        {
            if (debugMode) Debug.LogWarning("This UI is already displaying dialogue. Stopping previous dialogue.");
            StopDialogue(existingInstance);
        }

        // Create new instance
        DialogueInstance instance = new DialogueInstance(textTarget, boxTarget, textSpeed);
        instance.lines = dialogueLines;
        instance.onComplete = onComplete;
        instance.onCancel = onCancel;
        instance.isActive = true;
        instance.currentLineIndex = 0;
        
        activeInstances.Add(instance);
        
        if (instance.dialogueBoxImage != null)
            instance.dialogueBoxImage.enabled = true;
        
        DisplayLine(instance, instance.lines[instance.currentLineIndex]);
        
        return instance;
    }

    // Convenience methods for using default UI
    public DialogueInstance StartDialogue(string dialogueId, Action onComplete = null, Action onCancel = null)
    {
        return StartDialogue(dialogueId, defaultDialogueText, defaultDialogueBoxImage, onComplete, onCancel);
    }

    public DialogueInstance StartDialogue(List<string> dialogueLines, Action onComplete = null, Action onCancel = null)
    {
        return StartDialogue(dialogueLines, defaultDialogueText, defaultDialogueBoxImage, onComplete, onCancel);
    }

    // Force stop a specific dialogue (e.g., when player leaves area)
    public void StopDialogue(DialogueInstance instance)
    {
        if (instance == null || !activeInstances.Contains(instance)) return;
        
        if (instance.typingCoroutine != null)
        {
            StopCoroutine(instance.typingCoroutine);
        }
        
        if (instance.dialogueBoxImage != null)
            instance.dialogueBoxImage.enabled = false;
        
        instance.dialogueText.text = "";
        instance.isActive = false;
        
        activeInstances.Remove(instance);
        
        // Trigger cancel callback instead of complete
        instance.onCancel?.Invoke();
    }

    void DisplayLine(DialogueInstance instance, string line)
    {
        if (instance.typingCoroutine != null)
        {
            StopCoroutine(instance.typingCoroutine);
        }
        instance.typingCoroutine = StartCoroutine(TypeDialogue(instance, line));
    }

    void CompleteCurrentLine(DialogueInstance instance)
    {
        if (instance.typingCoroutine != null)
        {
            StopCoroutine(instance.typingCoroutine);
        }
        instance.dialogueText.text = instance.lines[instance.currentLineIndex];
        instance.isTyping = false;
    }

    void AdvanceDialogue(DialogueInstance instance)
    {
        instance.currentLineIndex++;
        
        if (instance.currentLineIndex < instance.lines.Count)
        {
            DisplayLine(instance, instance.lines[instance.currentLineIndex]);
        }
        else
        {
            EndDialogue(instance);
        }
    }

    void EndDialogue(DialogueInstance instance)
    {
        if (instance.dialogueBoxImage != null)
            instance.dialogueBoxImage.enabled = false;
        
        instance.isActive = false;
        instance.currentLineIndex = 0;
        instance.dialogueText.text = "";
        
        activeInstances.Remove(instance);
        
        instance.onComplete?.Invoke();
    }

    IEnumerator TypeDialogue(DialogueInstance instance, string line)
    {
        instance.isTyping = true;
        instance.dialogueText.text = "";
        
        foreach (char letter in line.ToCharArray())
        {
            instance.dialogueText.text += letter;
            yield return new WaitForSeconds(instance.textSpeed);
        }
        
        instance.isTyping = false;
    }

    DialogueInstance FindInstanceByUI(TextMeshProUGUI text, Image box)
    {
        return activeInstances.Find(i => i.dialogueText == text && i.dialogueBoxImage == box);
    }

    public bool IsAnyDialogueActive()
    {
        return activeInstances.Count > 0;
    }
    
    public bool IsDialogueActive(DialogueInstance instance)
    {
        return instance != null && instance.isActive && activeInstances.Contains(instance);
    }
}