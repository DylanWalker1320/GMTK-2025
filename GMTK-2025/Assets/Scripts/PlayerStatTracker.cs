using TMPro;
using UnityEngine;

public class PlayerStatTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statsText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void UpdateStatTrackerPanel(PlayerMovement player)
    {
        statsText.text = "Max Health: " + player.maxHealth.ToString() + 
                        "\nSpeed: " + player.maxSpeed.ToString() +
                        "\nIFrames: " + player.invincibilityFrames.ToString() + 
                        "\nCast Strength: " + (Mathf.Round(player.castStrength * 100.00f) * 0.01f).ToString() + 
                        "\nCast Speed: " + (Mathf.Round(player.castSpeed * 100.00f) * 0.01f).ToString();
    }
}
