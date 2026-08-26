using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using TMPro;

public class Portal : MonoBehaviour
{
    public Transform location = null;
    [SerializeField] private bool isReturnPortal = false; // Enable on the portal that brings the player to the main area

    private static CinemachineFollow ccamera;
    private GameManager gameManager;

    [SerializeField] private GameObject canvas;
    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private float proximityDistance = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private string dialogueLine;    
    [SerializeField]private TextMeshProUGUI dialogueText;
    public UIManager uiManager;
    public GameObject player;
    public bool isTyping = false;
    public bool playerInRange = false;
    public bool hasTriggered = false;

    void Start()
    {
        if (ccamera == null)
            ccamera = GameObject.FindGameObjectWithTag("MainCamera").transform.parent.gameObject.GetComponent<CinemachineFollow>();

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
            canvas.SetActive(true);
            hasTriggered = true;
        }

        // Interact trigger
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            if (uiManager.isInUI) return; // Prevent interaction if already in a UI
            
            ToggleTransitionUI();
        }

        // Player just left range
        if (distance > proximityDistance)
        {
            Debug.Log($"Player left portal range: {distance} > {proximityDistance}");
            hasTriggered = false;
            canvas.SetActive(false);
            dialogueText.text = string.Empty;

            StopAllCoroutines();
        }
    }

    public void LevelTransition()
    {
        FindAnyObjectByType<AudioManager>().Play("PORTALTRANSITION");

        ccamera.OnTargetObjectWarped(player.transform, location.position - player.transform.position);
        player.transform.position = location.position;

        // If this is the return portal, tell GameManager the player is back
        if (isReturnPortal && gameManager != null)
            gameManager.OnPlayerReturnedFromPortal();
        else if (gameManager != null)
            gameManager.playerInSafeArea = true;
    }

    void TypeDialogue(string line)
    {
        if (isTyping) StopAllCoroutines();

        StartCoroutine(TypeDialogueRoutine(line));
    }

    void ToggleTransitionUI()
    {
        StartCoroutine(EnableTransitionUICoroutine());
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

        // ccamera.OnTargetObjectWarped(other.transform, location.position - other.transform.position);
        // other.transform.position = location.position;

        // // If this is the return portal, tell GameManager the player is back
        // if (isReturnPortal && gameManager != null)
        //     gameManager.OnPlayerReturnedFromPortal();
        // else if (gameManager != null)
        //     Statue.TogglePurchaseAvailability(false);
        //     gameManager.playerInSafeArea = true;
        
        isTyping = false;
    }

    IEnumerator EnableTransitionUICoroutine()
    {
        Time.timeScale = 0f;
        uiManager.transitionUIAnimator.SetTrigger("TransitionBegin");
        yield return new WaitUntil(() => uiManager.transitionUIAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f);
        yield return new WaitWhile(() => uiManager.transitionUIAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f);
        LevelTransition();
        Time.timeScale = 1f;
        
    }
}