using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private bool debugMode;
    [Header("UI Panels")]
    public GameObject inventoryUI;
    public GameObject upgradeUI;
    public GameObject pauseMenu;
    public GameObject startMenu;
    public GameObject settingsMenu;
    public GameObject tutorialMenu;
    public GameObject barAllocationUI;
    // TODO: Add Level UP UI & Potentially Hat Roll UI
    [Header("Slides")]
    public GameObject Slide1;
    public GameObject Slide2;
    public GameObject Slide3;
    public GameObject Slide4;
    [Header("Health Bar")]
    public Slider healthBar;
    public TextMeshProUGUI healthBarText;
    [Header("Experience Bar")]
    public Slider experienceBar;
    public TextMeshProUGUI experienceBarText;
    public TextMeshProUGUI soulsText;
    [Header("Buttons")]
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;
    [SerializeField] private Button button4;
    [Header("UI State Management")]
    private bool isLevelingUp;
    [SerializeField] private Animator spellbarAllocationAnimator;
    [SerializeField] private Animator upgradeUIAnimator;
    // [SerializeField] private Animator spellUpgradeAnimator;
    public UnityEvent onShopFinish;

    private Menu lastMenu = Menu.StartMenu;
    private Menu currentMenu = Menu.StartMenu;

    private enum Menu
    {
        GameMenu,
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
    if(debugMode)
    {
        startMenu.SetActive(false);
        currentMenu = Menu.None;
        Time.timeScale = 1;
    }
    else
    {
        startMenu.SetActive(true);
        Time.timeScale = 0;
        currentMenu = Menu.StartMenu;
    }
}


    void Update()
    {
        if (!gameManager) return;

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
            else if (currentMenu == Menu.GameMenu)
            {
                pauseMenu.SetActive(true);
                lastMenu = currentMenu;
                currentMenu = Menu.PauseMenu;
            }
            else if (currentMenu == Menu.PauseMenu && lastMenu == Menu.None)
            {
                pauseMenu.SetActive(false);
                currentMenu = Menu.None;
                Time.timeScale = 1;
            }
            else if(currentMenu == Menu.PauseMenu && lastMenu == Menu.GameMenu)
            {
                pauseMenu.SetActive(false);
                currentMenu = Menu.GameMenu;
            }
        }

        if (gameManager.levelComplete && gameManager.isInSafeArea && !gameManager.loopComplete)
        {
            currentMenu = Menu.GameMenu;
            Time.timeScale = 0;
            SetActiveUpgradeUI();
            upgradeUI.GetComponent<ThreeUpgradeScreen>().UpdateDisplays();
        }
        else if (gameManager.loopComplete && gameManager.bossAlive == false)
        {
            currentMenu = Menu.GameMenu;
            Time.timeScale = 0;
            gameManager.loopComplete = false;
            SetActiveUpgradeUI();
            upgradeUI.GetComponent<ThreeUpgradeScreen>().UpdateDisplays();
            // INSERT HAT ROLL LOGIC HERE

            // isInShop = true;
            // gameManager.loopComplete = false;
            // NewGameLoopUpgradeUI();
            // newGameLoopUI.GetComponent<NewGameLoopMenu>().UpdateDisplays();
        }
    }

    public void UpdateHealthUI(float health, float maxHealth)
    {
        healthBar.value = health / maxHealth * 100;
        healthBarText.text = $"{health} / {maxHealth}";
    }

    public void UpdateExperienceUI(float currentEXP, float nextLevelEXP, int level, int souls)
    {
        float experiencePercentage = Mathf.Round(currentEXP / nextLevelEXP * 100.00f);
        experienceBar.value = experiencePercentage;
        experienceBarText.text = $"LV {level} {experiencePercentage}%";
        soulsText.text = souls.ToString();

    }


    private void SetButtonColor(Button button, Color color)
    {
        if (button != null)
        {
            var colors = button.colors;
            colors.normalColor = color;
            button.colors = colors;
        }
    }


    public void HandleSlideChange(string buttonPressed)
    {
        Debug.Log("Changing slide: " + buttonPressed);

        Slide1.SetActive(buttonPressed == "1");
        Slide2.SetActive(buttonPressed == "2");
        Slide3.SetActive(buttonPressed == "3");
        Slide4.SetActive(buttonPressed == "4");

        // Set all buttons to gray first
        SetButtonColor(button1, Color.gray);
        SetButtonColor(button2, Color.gray);
        SetButtonColor(button3, Color.gray);
        SetButtonColor(button4, Color.gray);

        // Set selected button to white
        switch (buttonPressed)
        {
            case "1": SetButtonColor(button1, Color.white); break;
            case "2": SetButtonColor(button2, Color.white); break;
            case "3": SetButtonColor(button3, Color.white); break;
            case "4": SetButtonColor(button4, Color.white); break;
        }


        Debug.Log("Changing slide:" + buttonPressed);
        if(buttonPressed == "1")
        {
            Slide1.SetActive(true);
            Slide2.SetActive(false);
            Slide3.SetActive(false);
            Slide4.SetActive(false);
        }
        if(buttonPressed == "2")
        {
            Slide1.SetActive(false);
            Slide2.SetActive(true);
            Slide3.SetActive(false);
            Slide4.SetActive(false);
        }
        if(buttonPressed == "3")
        {
            Slide1.SetActive(false);
            Slide2.SetActive(false);
            Slide3.SetActive(true);
            Slide4.SetActive(false);
        }
        if(buttonPressed == "4")
        {
            Slide1.SetActive(false);
            Slide2.SetActive(false);
            Slide3.SetActive(false);
            Slide4.SetActive(true);
        }
    }

    public void HandleBackToGameButton()
    {
        if(lastMenu == Menu.None) 
        {
            Time.timeScale = 1;
            currentMenu = Menu.None;
        }
        else if(lastMenu == Menu.GameMenu)
        {
            currentMenu = Menu.GameMenu;
        }
        pauseMenu.SetActive(false);
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
        // Debug.Log("HandleReturnToMainMenu | Returning to Start Menu from Pause Menu");

        // // Close all in-game UI
        // pauseMenu.SetActive(false);
        // settingsMenu.SetActive(false);
        // tutorialMenu.SetActive(false);
        // inventoryUI.SetActive(false);
        // upgradeUI.SetActive(false);

        // // Open the start menu
        // startMenu.SetActive(true);

        // // Update menu tracking
        // lastMenu = Menu.PauseMenu;
        // currentMenu = Menu.StartMenu;

        // // Stop the game time
        // Time.timeScale = 0;
        SceneManager.LoadScene("MainScene"); // Rework this section, but this'll serve for now long term
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
        upgradeUIAnimator.SetTrigger("BeginThreeUpgrades");
        // levelUpUI.SetActive(false);
        barAllocationUI.SetActive(false);
    }

    public void SetActiveLevelUpUI()
    {
        // levelUpUI.SetActive(!levelUpUI.activeSelf);
        // levelUpUI.GetComponent<SpellUpgradeUI>().UpdateExperience();
        upgradeUI.SetActive(false);
        barAllocationUI.SetActive(false);        
    }

    // public void SetActiveSpellUpgradeUI() // Delete this as soon as level up UI is fully functional
    // {
    //     levelUpUI.SetActive(!levelUpUI.activeSelf);
    //     levelUpUI.GetComponent<SpellUpgradeUI>().UpdateExperience();
    //     upgradeUI.SetActive(false);
    //     barAllocationUI.SetActive(false);
    // }

    // public void NewGameLoopUpgradeUI()
    // {
    //     newGameLoopUI.SetActive(!newGameLoopUI.activeSelf);
    //     upgradeUI.SetActive(false);
    //     levelUpUI.SetActive(false);
    //     barAllocationUI.SetActive(false);
    // }

    public void SetActiveBarAllocUI()
    {
        barAllocationUI.SetActive(!barAllocationUI.activeSelf);
        spellbarAllocationAnimator.SetTrigger("BeginSpellAllocation");
        upgradeUI.SetActive(false);
        if (barAllocationUI.activeSelf != false)
        {
            FindFirstObjectByType<InteractableLoopBar>().OnCall();
        }
    }

    public void GameplayMode() // Invoked as Unity Event
    {
        Time.timeScale = 1;
        currentMenu = Menu.None;
        upgradeUI.SetActive(false);
        // levelUpUI.SetActive(false);
        barAllocationUI.SetActive(false);
        // newGameLoopUI.SetActive(false);
        onShopFinish.Invoke();
        Debug.LogWarning("GameplayMode invoked");
    }
}
