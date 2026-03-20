using UnityEngine;
using TMPro;
using UnityEngine.UI;   

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private string dialogueId;
    
    [Header("UI Targets (leave empty to use default screen space UI)")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image dialogueBoxImage;
    
    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnProximity = false;
    [SerializeField] private float proximityDistance = 3f;
    [SerializeField] private bool triggerOnInteract = true;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private bool stopDialogueOnExit = true;
    [SerializeField] private float playerExitBufferArea = 2f; // Additional distance to prevent immediate stopping when player slightly steps out of range
    
    private Transform player;
    private bool playerInRange = false;
    private bool hasTriggered = false;
    private DialogueInstance currentDialogueInstance; // Track the active dialogue

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (dialogueBoxImage != null)
            dialogueBoxImage.enabled = false;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= proximityDistance;

        // Player just entered range
        if (triggerOnProximity && playerInRange && !hasTriggered)
        {
            TriggerDialogue();
            hasTriggered = true;
        }

        // Interact trigger
        if (triggerOnInteract && playerInRange && Input.GetKeyDown(interactKey))
        {
            TriggerDialogue();
        }

        // Player just left range
        if (distance > proximityDistance + playerExitBufferArea)
        {
            hasTriggered = false;
            
            // Stop dialogue if player leaves
            if (stopDialogueOnExit && currentDialogueInstance != null)
            {
                DialogueManager.Instance.StopDialogue(currentDialogueInstance);
                currentDialogueInstance = null;
            }
        }
    }

    public void TriggerDialogue()
    {
        if (!string.IsNullOrEmpty(dialogueId))
        {
            currentDialogueInstance = DialogueManager.Instance.StartDialogue(
                dialogueId, 
                dialogueText, 
                dialogueBoxImage, 
                OnDialogueComplete,
                OnDialogueCancel
            );
        }
    }

    void OnDialogueComplete()
    {
        Debug.Log("Dialogue Completed: " + dialogueId);
        currentDialogueInstance = null;
    }

    void OnDialogueCancel()
    {
        Debug.Log("Dialogue Cancelled: " + dialogueId);
        currentDialogueInstance = null;
    }

    void OnDrawGizmosSelected()
    {
        if (triggerOnProximity)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, proximityDistance);
            Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
            Gizmos.DrawWireSphere(transform.position, proximityDistance + playerExitBufferArea);
        }
    }
}