using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;
    public GameObject inventoryUI;
    public GameObject upgradeUI;
    public GameObject pauseMenu;
    public GameObject startMenu;
    public GameObject settingsMenu;
    public GameObject tutorialMenu;
    public GameObject spellUpgradeUI;
    public GameObject barAllocationUI;
    public GameObject newGameLoopUI;

    public bool isInShop;
    public UnityEvent onShopFinish;

    private Menu lastMenu = Menu.StartMenu;
    private Menu currentMenu = Menu.StartMenu;

    private enum Menu
    {
        StartMenu,
        PauseMenu,
        TutorialMenu,
        SettingsMenu,
        None
    }

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
        Time.timeScale = 0;
        currentMenu = Menu.StartMenu;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed");

            if (currentMenu == Menu.None) // Only allow pause if in game
            {
                pauseMenu.SetActive(true);
                lastMenu = currentMenu;
                currentMenu = Menu.PauseMenu;
                Time.timeScale = 0;
            }
            else if (currentMenu == Menu.PauseMenu)
            {
                pauseMenu.SetActive(false);
                currentMenu = Menu.None;
                Time.timeScale = 1;
            }
        }

        if (gameManager.levelComplete && !isInShop && !gameManager.loopComplete)
        {
            isInShop = true;
            SetActiveUpgradeUI();
            upgradeUI.GetComponent<ThreeUpgradeScreen>().UpdateDisplays();
        }
        else if (gameManager.loopComplete)
        {
            isInShop = true;
            gameManager.loopComplete = false;
            NewGameLoopUpgradeUI();
            newGameLoopUI.GetComponent<NewGameLoopMenu>().UpdateDisplays();
        }
    }

    public void NewGameLoopUpgradeUI()
    {
        newGameLoopUI.SetActive(!newGameLoopUI.activeSelf);
        upgradeUI.SetActive(false);
        spellUpgradeUI.SetActive(false);
        barAllocationUI.SetActive(false);
    }

    public void HandleBackToGameButton()
    {
        pauseMenu.SetActive(false);
        currentMenu = Menu.None;
        Time.timeScale = 1;
    }
    public void HandleSettingsButton()
    {
        Debug.Log("HandleSettingsButton | Last: " + lastMenu + ", Current: " + currentMenu);
        lastMenu = currentMenu;
        currentMenu = Menu.SettingsMenu;

        settingsMenu.SetActive(true);
        pauseMenu.SetActive(false);
        startMenu.SetActive(false);
        tutorialMenu.SetActive(false);
    }

    public void HandleStartButton()
    {
        Debug.Log("HandleStartButton | Last: " + lastMenu + ", Current: " + currentMenu);
        lastMenu = currentMenu;
        currentMenu = Menu.None; // None = in-game

        startMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void HandleTutorialButton()
    {
        Debug.Log("HandleTutorialButton | Last: " + lastMenu + ", Current: " + currentMenu);
        lastMenu = currentMenu;
        currentMenu = Menu.TutorialMenu;

        tutorialMenu.SetActive(true);
        pauseMenu.SetActive(false);
        startMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    public void HandleBackButton()
    {
        Debug.Log("HandleBackButton | Returning to: " + lastMenu);
        
        // Disable current menu
        settingsMenu.SetActive(false);
        tutorialMenu.SetActive(false);

        // Return to last menu
        currentMenu = lastMenu;

        switch (lastMenu)
        {
            case Menu.StartMenu:
                startMenu.SetActive(true);
                break;
            case Menu.PauseMenu:
                pauseMenu.SetActive(true);
                break;
        }
    }

    public void HandleReturnToMainMenu()
    {
        Debug.Log("HandleReturnToMainMenu | Returning to Start Menu from Pause Menu");

        // Close all in-game UI
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        tutorialMenu.SetActive(false);
        inventoryUI.SetActive(false);
        upgradeUI.SetActive(false);

        // Open the start menu
        startMenu.SetActive(true);

        // Update menu tracking
        lastMenu = Menu.PauseMenu;
        currentMenu = Menu.StartMenu;

        // Stop the game time
        Time.timeScale = 0;
    }


    public void HandleQuitButton()
    {
        Debug.Log("HandleQuitButton");
        Application.Quit();
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
        newGameLoopUI.SetActive(false);
        gameManager.isInSafeArea = false;
        onShopFinish.Invoke();
    }
}
