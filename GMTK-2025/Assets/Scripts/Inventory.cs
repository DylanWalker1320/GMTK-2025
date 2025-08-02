using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerMovement player;
    private SpellCombinations spellCombinations;
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
        gameManager = FindFirstObjectByType<GameManager>();
        if (player == null)
        {
            Debug.LogError("PlayerMovement not found in the scene.");
        }
        spellCombinations = FindFirstObjectByType<SpellCombinations>();
        timeBetweenSpells = maxTimeBetweenSpells; // Initialize time between spells
    }
    private void Start()
    {
        GetSpellSprites(); // Get the spell sprites for the inventory slots
    }
    private void Update()
    {
        timeBetweenSpells -= Time.deltaTime * player.castSpeed; // Decrease the time between spells
        if (!isCasting && !gameManager.isInSafeArea)
        {
            Cast();
        }
        if (timeBetweenSpells < 0f && !gameManager.isInSafeArea)
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
        if (spellArray[currentSpellIndex] != null)
        {
            Instantiate(spellArray[currentSpellIndex], player.reticle.GetChild(0).position, Quaternion.Euler(0f, 180f, 0f));
        }
        inventorySlots[currentSpellIndex].color = Color.gray; // Change the color of the slot to indicate casting
    }

    public void Loop() // loop through inventory slots
    {
        inventorySlots[currentSpellIndex].color = Color.white;
        currentSpellIndex++; // Increment the spell index
        timeBetweenSpells = maxTimeBetweenSpells; // Reset the time between spells
        isCasting = false; // Reset casting flag after casting
    }

    private void GetSpellSprites()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (spellArray[i] == null)
            {
                Debug.Log("Spell is null, skipping check.");
                continue; // Skip if either spell is null
            }
            //Spell Combo Check
            else if (i % 2 == 0 && i < spellArray.Length - 1 && spellArray[i+ 1] != null) // Check Combination using first slot of the pair (change this up later)
            {
                Spell comboSpell = spellCombinations.OutputSpellCombination(spellArray[i], spellArray[i + 1]);
                if (comboSpell != null)
                {
                    spellArray[i] = comboSpell; // Assign the combination spell to the current slot
                    spellArray[i + 1] = comboSpell; // Assign the same combination spell to the next slot
                    inventorySlots[i].sprite = comboSpell.spellSprite;
                    inventorySlots[i + 1].sprite = comboSpell.spellSprite;
                    i++; // Skip the next slot since it's part of the combination
                }
                else
                {
                    Debug.Log("Normal Spell");
                    inventorySlots[i].sprite = spellArray[i].spellSprite;
                }
            }
            else // Should patch this later for cleaner but it works for now. Covers the odd index slots if no combo
            {
                Debug.Log("Normal Spell");
                inventorySlots[i].sprite = spellArray[i].spellSprite;
            }
        }
    }
}
