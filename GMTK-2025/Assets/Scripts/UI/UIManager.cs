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
    public GameObject upgradeUI;
    public GameObject statShopUI; // TODO: now stat shop ui, rename later
    public GameObject scrollUI;
    public GameObject barAllocationUI;
    public GameObject statTrackerUI;
    public GameObject spellBookUI;
    public GameObject pauseMenu;
    public GameObject startMenu;
    public GameObject settingsMenu;
    public GameObject tutorialMenu;
    [Header("UI Animations")]
    public float animationDuration = 0.50f; // Duration of UI animations
    public GameObject transitionUI;
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
    public Animator spellbarAllocationAnimator;
    [SerializeField] private Animator upgradeUIAnimator;
    // [SerializeField] private Animator spellUpgradeAnimator;
    public UnityEvent onShopFinish;

    private Menu lastMenu = Menu.StartMenu;
    private Menu currentMenu = Menu.StartMenu;

    public bool isInUI = false; // Flag to track if any UI is currently active

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

        if (Input.GetKeyDown(KeyCode.Escape) && statShopUI.activeSelf == false && scrollUI.activeSelf == false) // Don't allow pause if we're in the middle of leveling up or rolling for hats
        {

            if (currentMenu == Menu.None) // Only allow pause if in game
            {
                pauseMenu.SetActive(true);
                FindAnyObjectByType<AudioManager>().Play("OpenPauseMenu");
                pauseMenu.GetComponent<Animator>().SetTrigger("BeginPauseMenu");
                lastMenu = currentMenu;
                currentMenu = Menu.PauseMenu;
                Time.timeScale = 0;
            }
            else if (currentMenu == Menu.GameMenu) // look into this it might be useless ngl
            {
                pauseMenu.SetActive(true);
                FindAnyObjectByType<AudioManager>().Play("OpenPauseMenu");
                pauseMenu.GetComponent<Animator>().SetTrigger("BeginPauseMenu");
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
        else if(Input.GetKeyDown(KeyCode.C))
        {
            statTrackerUI.SetActive(!statTrackerUI.activeSelf);
            if(statTrackerUI.activeSelf)
            {
                updateStatTrackerUI(); // panel needs to update with current data once player toggles
            }
        }
        
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if((currentMenu == Menu.GameMenu || currentMenu == Menu.None) && scrollUI.activeSelf == false)
            {
                spellBookUI.SetActive(!spellBookUI.activeSelf);
            }
        }
        // TODO: another conditonal statement to trigger Spell Swap UI and pause time. Can only be triggered during gameplay. Goal is to reuse Spell Bar Allocation UI for this, enum checking will handle logic for whether we're swapping or allocating along its display. 
        // If pressed again, should exit out of the UI and unpause time. To be called in UIManager

        else if (Input.GetKeyDown(KeyCode.Tab) && statShopUI.activeSelf == false && scrollUI.activeSelf == false && upgradeUI.activeSelf == false && barAllocationUI.activeSelf == false)
        {
            if(currentMenu == Menu.GameMenu || currentMenu == Menu.None)
            {
                Time.timeScale = 0;
                SetActiveBarAllocUI(InteractableLoopBar.LoopBarType.SpellSwap); // Opens the spell swap UI, which reuses the bar allocation UI
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && statShopUI.activeSelf == false && scrollUI.activeSelf == false && barAllocationUI.activeSelf == true && FindFirstObjectByType<InteractableLoopBar>().loopBarType == InteractableLoopBar.LoopBarType.SpellSwap)
        {
            if(currentMenu == Menu.GameMenu || currentMenu == Menu.None)
            {
                Time.timeScale = 1;
                SetActiveBarAllocUI(InteractableLoopBar.LoopBarType.SpellSwap); // Closes the spell swap UI, which reuses the bar allocation UI
            }
        }

        if (gameManager.levelComplete && gameManager.isInSafeArea && !gameManager.loopComplete)
        {
            currentMenu = Menu.GameMenu;
            Time.timeScale = 0;
            SetActiveUpgradeUI();
            upgradeUI.GetComponent<ThreeUpgradeScreen>().UpdateDisplays();
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

    public void updateStatTrackerUI()
    {
        if(statTrackerUI.activeSelf)
        {
            statTrackerUI.GetComponent<PlayerStatTracker>().UpdateStatTrackerPanel(FindAnyObjectByType<PlayerMovement>());
        }
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
        SceneManager.LoadScene("MainMenu");
    }


    public void HandleQuitButton()
    {
        Debug.Log("HandleQuitButton");
        Application.Quit();
    }

    public void SetBetaGameplay()
    {
        Time.timeScale = 0;

        FindAnyObjectByType<Inventory>().WipeInventory();

        barAllocationUI.SetActive(true);
        FindAnyObjectByType<InteractableLoopBar>().BetaLoop(); // needs above to be active in the first place
    }

    public void SetActiveUpgradeUI()
    {
        if (isInUI) return; // Prevent opening if already in a UI
        isInUI = true;
        upgradeUI.SetActive(!upgradeUI.activeSelf);
        upgradeUIAnimator.SetTrigger("BeginThreeUpgrades");
        barAllocationUI.SetActive(false);
    }

    public void SetActiveStatShopUI()
    {
        if (isInUI) return; // Prevent opening if already in a UI
        isInUI = true;
        statShopUI.SetActive(!statShopUI.activeSelf);
        upgradeUI.SetActive(false);
        barAllocationUI.SetActive(false);

        Time.timeScale = 0;
        statShopUI.GetComponent<LevelUpUI>().InitializeStatShopUI();
    }

    public void SetActiveScrollUI()
    {
        if (isInUI) return; // Prevent opening if already in a UI
        isInUI = true;
        Time.timeScale = 0;        
        isLevelingUp = true;
        scrollUI.SetActive(!scrollUI.activeSelf);
        scrollUI.GetComponent<HatScrollUI>().ToggleScrollUI(newInitialization: true);
        upgradeUI.SetActive(false);
        barAllocationUI.SetActive(false);
    }

    public void SetActiveBarAllocUI(InteractableLoopBar.LoopBarType loopBarType)
    {
        barAllocationUI.SetActive(!barAllocationUI.activeSelf);
        upgradeUI.SetActive(false);


        if (barAllocationUI.activeSelf != false)
        {
            isInUI = true;
            FindFirstObjectByType<InteractableLoopBar>().loopBarType = loopBarType;
            FindFirstObjectByType<InteractableLoopBar>().OnCall();
        }
        else
        {
            isInUI = false;
            TooltipManager._instance.HideTooltip();
        }
    }

    public void GameplayMode() // Invoked as Unity Event
    {
        isInUI = false;
        Time.timeScale = 1;
        currentMenu = Menu.None;
        upgradeUI.SetActive(false);
        statShopUI.SetActive(false);
        barAllocationUI.SetActive(false);
        scrollUI.SetActive(false);
        
        if (!isLevelingUp && !gameManager.waitingForPortalReturn)
        {
            onShopFinish.Invoke();
        }
        isLevelingUp = false;
        gameManager.SetEnemyPause(false);
        Debug.LogWarning("GameplayMode invoked");
    }
}
