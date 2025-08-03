using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager; // Reference to the GameManager script
    public GameObject inventoryUI; // Reference to the inventory UI panel
    public GameObject upgradeUI; // Reference to the upgrade UI panel
                                 // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject spellUpgradeUI;
    public GameObject barAllocationUI;

    public bool isInShop;
    public UnityEvent onShopFinish;
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
        spellUpgradeUI.SetActive(false);
        barAllocationUI.SetActive(false);
    }

    // Last Shop Panel
    public void SetActiveSpellUpgradeUI()
    {
        spellUpgradeUI.SetActive(!spellUpgradeUI.activeSelf);
        spellUpgradeUI.GetComponent<SpellUpgradeUI>().UpdateExperience();
        upgradeUI.SetActive(false);
        barAllocationUI.SetActive(false);
    }

    public void SetActiveBarAllocUI()
    {
        barAllocationUI.SetActive(!barAllocationUI.activeSelf);
        upgradeUI.SetActive(false);
        if (barAllocationUI.activeSelf != false)
        {
            FindFirstObjectByType<InteractableLoopBar>().OnCall();
        }
    }

    public void GameplayMode()
    {
        upgradeUI.SetActive(false);
        spellUpgradeUI.SetActive(false);
        barAllocationUI.SetActive(false);
        onShopFinish.Invoke();
    }
}
