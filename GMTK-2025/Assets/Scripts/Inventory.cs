using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private TopDownPlayerMovement player;
    public Image[] inventorySlots = new Image[8]; // UI slots for spells
    public GameObject[] spellArray = new GameObject[8]; // Holds spell prefabs, consider changing prefabs to be of spell type
    [SerializeField] private float maxTimeBetweenSpells;
    private float timeBetweenSpells; // Time between casting spells
    public bool isPlayerAlive = true; // Flag to check if the player is active
    public bool isCasting = false;
    public int currentSpellIndex = 0; // Index of the current spell being cast
    private void Awake()
    {
        player = FindFirstObjectByType<TopDownPlayerMovement>();
        if (player == null)
        {
            Debug.LogError("TopDownPlayerMovement not found in the scene.");
        }
        timeBetweenSpells = maxTimeBetweenSpells; // Initialize time between spells
    }
    private void Update()
    {
        timeBetweenSpells -= Time.deltaTime; // Decrease the time between spells
        if (!isCasting)
        {
            Loop();
        }
        if (timeBetweenSpells < 0f)
        {
            inventorySlots[currentSpellIndex].color = Color.white;
            currentSpellIndex++; // Increment the spell index
            timeBetweenSpells = maxTimeBetweenSpells; // Reset the time between spells
            isCasting = false; // Reset casting flag after casting
        }
    }

    public void Loop()
    {
        isCasting = true; // Set casting to true to prevent multiple casts
        if (currentSpellIndex == spellArray.Length)
        {
            currentSpellIndex = 0; // Reset to the first spell if we exceed the array length
        }
        player.CastMagic(spellArray[currentSpellIndex]);
        inventorySlots[currentSpellIndex].color = Color.gray; // Change the color of the slot to indicate casting
    }
}
