using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using TMPro;

public class Portal : MonoBehaviour
{
    public Transform location = null;
    [SerializeField] private bool isReturnPortal = false; // Enable on the portal that brings the player to the main area

    private static CinemachineFollow camera;
    private GameManager gameManager;

    [SerializeField] private GameObject canvas;
    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private float proximityDistance = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private string dialogueLine;    
    [SerializeField]private TextMeshProUGUI dialogueText;
    private UIManager uiManager;
    private GameObject player;
    private bool isTyping = false;
    private bool playerInRange = false;
    private bool hasTriggered = false;

    void Start()
    {
        if (camera == null)
            camera = GameObject.FindGameObjectWithTag("MainCamera").transform.parent.gameObject.GetComponent<CinemachineFollow>();

        gameManager = FindFirstObjectByType<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        uiManager = FindFirstObjectByType<UIManager>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        playerInRange = distance <= proximityDistance;

        // Player just entered range
        if (playerInRange && !hasTriggered)
        {
            TypeDialogue(dialogueLine);
            hasTriggered = true;
        }

        // Interact trigger
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            if (uiManager.isInUI) return; // Prevent interaction if already in a UI

            FindAnyObjectByType<AudioManager>().Play("PORTALTRANSITION");

            camera.OnTargetObjectWarped(player.transform, location.position - player.transform.position);
            player.transform.position = location.position;

            // If this is the return portal, tell GameManager the player is back
            if (isReturnPortal && gameManager != null)
                gameManager.OnPlayerReturnedFromPortal();
            else if (gameManager != null)
                gameManager.playerInSafeArea = true;
        }

        Debug.Log($"Player distance from statue: {distance}, proximity threshold: {proximityDistance}, player in range: {playerInRange}");
        // Player just left range
        if (distance > proximityDistance)
        {
            hasTriggered = false;
            canvas.SetActive(false);
            dialogueText.text = string.Empty;

            StopAllCoroutines();
        }
    }

    void TypeDialogue(string line)
    {
        if (isTyping) StopAllCoroutines();

        StartCoroutine(TypeDialogueRoutine(line));
    }

    IEnumerator TypeDialogueRoutine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        canvas.SetActive(true);
        
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }
}