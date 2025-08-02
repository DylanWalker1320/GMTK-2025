using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject inventoryUI; // Reference to the inventory UI panel
    public GameObject upgradeUI; // Reference to the upgrade UI panel
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            // upgradeUI.SetActive(!upgradeUI.activeSelf); // Toggle the visibility of the upgrade UI
    }
}
