using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerMovement player;
    private SpellCombinations spellCombinations;
    private AudioManager audioManager;
    public Image[] inventorySlots = new Image[8]; // UI slots for spells
    public Spell[] spellArray = new Spell[8]; // Holds spell prefabs, consider changing prefabs to be of spell type
    [SerializeField] private float maxTimeBetweenSpells;
    public Spell chosenSpell;
    private float timeBetweenSpells; // Time between casting spells
    public bool isCasting = false; // casting current spell
    public int currentSpellIndex = 0; // Index of the current spell being cast
    private void Awake()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        gameManager = FindFirstObjectByType<GameManager>();
        audioManager = FindFirstObjectByType<AudioManager>();
        if (player == null)
        {
            Debug.LogError("PlayerMovement not found in the scene.");
        }
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found in the scene.");
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

            // Play sound effect based on spell name
            if (audioManager != null)
            {
                string spellName = spellArray[currentSpellIndex].name;
                audioManager.Play(spellName);
            }
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

    public void GetSpellSprites()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (spellArray[i] == null)
            {
                Debug.Log("Spell is null, skipping check.");
                continue; // Skip if either spell is null
            }
            else
            {
                inventorySlots[i].sprite = spellArray[i].spellSprite;
            }
        }
    }

    public bool CheckNewElementSelection(int index)
    {
        bool isSingle = false;
        if (spellArray[index] != null)
        {
            Spell comboSpell = spellCombinations.OutputSpellCombination(spellArray[index], chosenSpell);
            isSingle = true;
            spellArray[index] = comboSpell; // Assign the combination spell to the current slot
            inventorySlots[index].sprite = comboSpell.spellSprite;
            GetSpellSprites();
        }
        else
        {
            spellArray[index] = chosenSpell;
            inventorySlots[index].sprite = chosenSpell.spellSprite;   
        }

        return isSingle;
    }
}
