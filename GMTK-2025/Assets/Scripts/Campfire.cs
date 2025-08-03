using UnityEngine;

public class Campfire : MonoBehaviour
{
    private UIManager uiManager; // Reference to the UIManager script

    void Awake()
    {
        uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager not found in the scene.");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            uiManager.upgradeUI.SetActive(true); // Show the upgrade UI when the player enters the campfire area
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            uiManager.upgradeUI.SetActive(false); // Hide the upgrade UI when the player exits the campfire area
        }
    }

}
