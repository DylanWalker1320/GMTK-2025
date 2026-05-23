using UnityEngine;
using TMPro;
using System.Collections;

public class Statue : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject canvas;
    [SerializeField] private StatueType statueType;
    [SerializeField] private float basePrice;
    [SerializeField] private float price;
    [SerializeField] private float priceIncreaseRate;
    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private float proximityDistance = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private UIManager uiManager;
    private string dialogueLine;
    private bool isTyping = false;
    private bool playerInRange = false;
    private bool hasTriggered = false;
    private bool hasPurchased = false;
    private string soulColourPrefix = "<color=#6BFFCB>";
    private string soulColourSuffix = "</color>";
    private char soulPrefixSymbol = '▒';
    private char soulSuffixSymbol = '▓';
    private int playerSouls;

    [SerializeField]private TextMeshProUGUI dialogueText;

    
    enum StatueType
    {
        Hat,
        Stat
    }

    public static void ResetPrices()
    {
        foreach (Statue statue in FindObjectsOfType<Statue>())
        {
            statue.price = statue.basePrice;
            statue.UpdatePrice();
        }
    }

    public static void TogglePurchaseAvailability(bool purchaseCondition)
    {
        foreach (Statue statue in FindObjectsByType<Statue>(FindObjectsSortMode.None))
        {
            if (statue.statueType == StatueType.Stat)
            {
                statue.hasPurchased = purchaseCondition;
            }
        }
    }

    void Start()
    {
        canvas.SetActive(false);
        price = basePrice;

        UpdatePrice(); // Initialize the dialogue line with the starting price

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (dialogueText == null)
        {
            dialogueText = canvas.GetComponentInChildren<TextMeshProUGUI>();
        }
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

            if(hasPurchased && statueType == StatueType.Stat) return; // Prevent multiple interactions without leaving range

            playerSouls = player.GetComponent<PlayerMovement>()?.souls ?? 0; // Get player's current souls
            if (playerSouls >= price)
            {
                playerSouls -= (int)price; // Deduct souls from player
                PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
                playerMovement.souls = playerSouls; // Update player's soul count
                playerMovement.UpdateUI(); // Update UI to reflect new soul count
                price *= priceIncreaseRate; // Increase price for next purchase

                switch (statueType)
                {
                    case StatueType.Hat:
                        uiManager.SetActiveScrollUI();
                        break;
                    case StatueType.Stat:
                        uiManager.SetActiveStatShopUI();
                        break;
                }

                UpdatePrice(); // Update the dialogue text with the new price

                TypeDialogue($"Purchase successful!");
            }
            else
            {
                TypeDialogue($"Not enough souls!");
            }
        }

        // Player just left range
        if (distance > proximityDistance)
        {
            hasTriggered = false;
            canvas.SetActive(false);
            dialogueText.text = string.Empty;

            StopAllCoroutines();
        }
    }

    void UpdatePrice()
    {
        switch (statueType)
        {
            case StatueType.Hat:
                dialogueLine = $"Press 'E' to buy a hat for {soulPrefixSymbol}{price} souls{soulSuffixSymbol}!";
                break;
            case StatueType.Stat:
                dialogueLine = $"Press 'E' to increase your stats for {soulPrefixSymbol}{price} souls{soulSuffixSymbol}!";
                break;
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
            if (letter == soulPrefixSymbol)
            {
                dialogueText.text += soulColourPrefix;
            }
            else if (letter == soulSuffixSymbol)
            {
                dialogueText.text += soulColourSuffix;
            }
            else
            {
                dialogueText.text += letter;
            }
            
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
    }
}
