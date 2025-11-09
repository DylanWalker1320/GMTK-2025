using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class InteractableLoopBar : MonoBehaviour
{
    private Inventory loopbarInventory;
    private GameManager gameManager;
    public Image[] inventorySlots = new Image[8]; // UI slots for spells
    public Spell[] spellArray = new Spell[8]; // Holds spell prefabs, consider changing prefabs to be of spell type
    private SpellCombinations spellCombinations;
    [SerializeField] private TextMeshProUGUI[] typeText = new TextMeshProUGUI[8];
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
        spellArray = loopbarInventory.spellArray; //pointer for actual spell array
        GetSpellSprites();
        GetTypes();
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
            else
            {
                inventorySlots[i].sprite = spellArray[i].spellSprite;
            }
        }
    }

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
                loopbarInventory.chosenSpell = spellReplacements[0];
                break;
            case Spell.SpellType.Water:
                loopbarInventory.chosenSpell = spellReplacements[1];
                break;
            case Spell.SpellType.Lightning:
                loopbarInventory.chosenSpell = spellReplacements[2];
                break;
            case Spell.SpellType.Dark:
                loopbarInventory.chosenSpell = spellReplacements[3];
                break;
        }
        TransitionToMagicUpgradeScreen(index);
    }

    public void TransitionToMagicUpgradeScreen(int index)
    {
        bool isSingle;
        isSingle = loopbarInventory.CheckNewElementSelection(index);
        if (!isSingle)
        {
            GetSpellSprites();
        }

        unityEvent.Invoke();
    }
}
