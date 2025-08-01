using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private PlayerMovement player;
    public Image[] inventorySlots = new Image[8]; // UI slots for spells
    public Spell[] spellArray = new Spell[8]; // Holds spell prefabs, consider changing prefabs to be of spell type
    [SerializeField] private float maxTimeBetweenSpells;
    private float timeBetweenSpells; // Time between casting spells
    public bool isPlayerAlive = true; // Flag to check if the player is active
    public bool isCasting = false; // casting current spell
    public int currentSpellIndex = 0; // Index of the current spell being cast
    private void Awake()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        if (player == null)
        {
            Debug.LogError("PlayerMovement not found in the scene.");
        }
        timeBetweenSpells = maxTimeBetweenSpells; // Initialize time between spells
    }
    private void Start()
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].sprite = spellArray[i].spellSprite; // Set the sprite for each inventory slot
        }
    }
    private void Update()
    {
        timeBetweenSpells -= Time.deltaTime; // Decrease the time between spells
        if (!isCasting)
        {
            Cast();
        }
        if (timeBetweenSpells < 0f)
        {
            Loop();
        }
    }

    public void Cast() // Casting current spell ability
    {
        isCasting = true; // Set casting to true to prevent multiple casts
        if (currentSpellIndex == spellArray.Length)
        {
            currentSpellIndex = 0; // Reset to the first spell if we exceed the array length
        }
        Instantiate(spellArray[currentSpellIndex], player.reticle.GetChild(0).position, Quaternion.Euler(0f, 180f, 0f));
        inventorySlots[currentSpellIndex].color = Color.gray; // Change the color of the slot to indicate casting
    }

    public void Loop() // loop through inventory slots
{
        inventorySlots[currentSpellIndex].color = Color.white;
        currentSpellIndex++; // Increment the spell index
        timeBetweenSpells = maxTimeBetweenSpells; // Reset the time between spells
        isCasting = false; // Reset casting flag after casting
    }
}
