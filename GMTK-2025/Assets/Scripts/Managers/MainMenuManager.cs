using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button firstSelectableButton;
    [SerializeField] private PlayerInput playerInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1; // Ensure the game time is running when returning to the main menu
    }

    void Update()
    {
        
    }


    public void StartGame()
    {
        // Load the game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
    private void OnEnable()
    {
        playerInput.onControlsChanged += OnControlsChanged;
    }

    private void OnDisable()
    {
        playerInput.onControlsChanged -= OnControlsChanged;
    }

    private void OnControlsChanged(PlayerInput playerInput)
    {
        if(playerInput.currentControlScheme == "Controller")
        {
            SelectButton();
        }

        if(playerInput.currentControlScheme == "KeyboardMouse")
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    private void SelectButton()
    {
        if (firstSelectableButton == null) {return;}
        Cursor.lockState = CursorLockMode.Locked;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectableButton.gameObject);
    }
}
