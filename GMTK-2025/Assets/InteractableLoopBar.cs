using UnityEngine;
using UnityEngine.UI;

public class InteractableLoopBar : MonoBehaviour
{
    private Inventory loopbarInventory;
    public Image[] inventorySlots = new Image[8]; // UI slots for spells
    public Spell[] spellArray = new Spell[8]; // Holds spell prefabs, consider changing prefabs to be of spell type
    private SpellCombinations spellCombinations;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void OnCall()
    {
        spellCombinations = FindFirstObjectByType<SpellCombinations>();
        loopbarInventory = FindFirstObjectByType<Inventory>();
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
}
