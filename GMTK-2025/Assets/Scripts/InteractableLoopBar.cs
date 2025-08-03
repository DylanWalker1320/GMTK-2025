using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InteractableLoopBar : MonoBehaviour
{
    private Inventory loopbarInventory;
    private GameManager gameManager;
    public Image[] inventorySlots = new Image[8]; // UI slots for spells
    public Spell[] spellArray = new Spell[8]; // Holds spell prefabs, consider changing prefabs to be of spell type
    private SpellCombinations spellCombinations;
    [SerializeField] private Image spellImage;
    [SerializeField] private Spell[] spellReplacements = new Spell[4];

    [SerializeField] private UnityEvent unityEvent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void OnCall()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        spellCombinations = FindFirstObjectByType<SpellCombinations>();
        loopbarInventory = FindFirstObjectByType<Inventory>();
        spellImage.sprite = gameManager.spellImage;
        spellArray = loopbarInventory.spellArray;
        GetSpellSprites();
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
            else if (i % 2 == 0 && i < spellArray.Length - 1 && spellArray[i + 1] != null) // Check Combination using first slot of the pair (change this up later)
            {
                if (spellArray[i].spellType2 == Spell.SpellType.None)
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
                        inventorySlots[i].sprite = spellArray[i].spellSprite;
                    }
                }
                else
                {
                    inventorySlots[i].sprite = spellArray[i].spellSprite;
                }
            }
            else // Should patch this later for cleaner but it works for now. Covers the odd index slots if no combo
            {
                inventorySlots[i].sprite = spellArray[i].spellSprite;
            }
        }
    }
<<<<<<< Updated upstream
=======
    private void GetTypes()
    {
        for (int i = 0; i < typeText.Length; i++)
        {
            if (spellArray[i] != null)
            {
                switch (spellArray[i].spellType1)
                {
                    case Spell.SpellType.Fire:
                        typeText[i].text = "Fire";
                        typeText[i].color = Color.red;
                        break;
                    case Spell.SpellType.Water:
                        typeText[i].text = "Water";
                        typeText[i].color = Color.blue;
                        break;
                    case Spell.SpellType.Lightning:
                        typeText[i].text = "Lightning";
                        typeText[i].color = Color.yellow;
                        break;
                    case Spell.SpellType.Dark:
                        typeText[i].text = "Dark";
                        typeText[i].color = Color.gray;
                        break;
                    default:
                        typeText[i].text = "";
                        break;

                }
            }

        }
    }
>>>>>>> Stashed changes
    // Button Functions
    public void SlotOne()
    {
        SelectSpellReplacement(0);
    }
    public void SlotTwo()
    {
        SelectSpellReplacement(1);
    }
    public void SlotThree()
    {
        SelectSpellReplacement(2);
    }
    public void SlotFour()
    {
        SelectSpellReplacement(3);
    }
    public void SlotFive()
    {
        SelectSpellReplacement(4);
    }
    public void SlotSix()
    {
        SelectSpellReplacement(5);
    }
    public void SlotSeven()
    {
        SelectSpellReplacement(6);
    }
    public void SlotEight()
    {
        SelectSpellReplacement(7);

    }

    void SelectSpellReplacement(int index)
    {
        switch (gameManager.allocateSpell)
        {
            case Spell.SpellType.Fire:
                spellArray[index] = spellReplacements[0];
                break;
            case Spell.SpellType.Water:
                spellArray[index] = spellReplacements[1];
                break;
            case Spell.SpellType.Lightning:
                spellArray[index] = spellReplacements[2];
                break;
            case Spell.SpellType.Dark:
                spellArray[index] = spellReplacements[3];
                break;
        }
        TransitionToMagicUpgradeScreen(index);
    }

    public void TransitionToMagicUpgradeScreen(int index)
    {

        loopbarInventory.spellArray = spellArray;
        loopbarInventory.CheckNewElementSelection(index);
        unityEvent.Invoke();
    }
}
