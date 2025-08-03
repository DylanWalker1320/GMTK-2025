using UnityEngine;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager; // Reference to the GameManager script
    public GameObject inventoryUI; // Reference to the inventory UI panel
    public GameObject upgradeUI; // Reference to the upgrade UI panel
                                 // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject spellUpgradeUI;
    public GameObject barAllocationUI;

    [SerializeField] private bool isInShop;
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.levelComplete && !isInShop)
        {
            isInShop = true;
            SetActiveUpgradeUI();
            upgradeUI.GetComponent<ThreeUpgradeScreen>().UpdateDisplays();

        }

    }

    public void SetActiveInventoryUI()
    {
        inventoryUI.SetActive(!upgradeUI.activeSelf);
    }

    public void SetActiveUpgradeUI()
    {
        upgradeUI.SetActive(!upgradeUI.activeSelf);
    }

    // Last Shop Panel
    public void SetActiveSpellUpgradeUI()
    {
        spellUpgradeUI.SetActive(!spellUpgradeUI.activeSelf);
        spellUpgradeUI.GetComponent<SpellUpgradeUI>().UpdateExperience();
    }

    public void SetActiveBarAllocUI()
    {
        barAllocationUI.SetActive(!barAllocationUI.activeSelf);
        if (barAllocationUI.activeSelf != false)
        {
            FindFirstObjectByType<InteractableLoopBar>().OnCall();   
        }
    }
}
