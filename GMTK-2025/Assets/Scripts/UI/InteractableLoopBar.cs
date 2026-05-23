using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class InteractableLoopBar : MonoBehaviour
{
    private Inventory loopbarInventory;
    private GameManager gameManager;
    private int startingSpellCounter;
    public Image[] inventorySlots = new Image[8]; // UI slots for spells
    public Spell[] spellArray = new Spell[8];
    public LoopBarType loopBarType;
    public SwapState swapState;
    [SerializeField] private TextMeshProUGUI[] typeText = new TextMeshProUGUI[8];
    [SerializeField] private Image spellImage;
    [SerializeField] private Spell[] spellReplacements = new Spell[4];

    [SerializeField] private UnityEvent unityEvent;

    [SerializeField] int spellToSwapOneIndex;
    [SerializeField] int spellToSwapTwoIndex;

    [SerializeField] private Sprite emptySlotSprite; // Sprite for empty inventory slots
    private Spell spellToSwapOne;
    private Spell spellToSwapTwo;


    //TODO: Create an enum class for interactable loop bar type: Spell Swap, SpellCombination. Use enums to determine whether the slots switch or replace/combine the spell

    public enum LoopBarType
    {
        SpellSwap,
        SpellCombination
    }

    public enum SwapState
    {
        None,
        FirstSelected
    }

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        loopbarInventory = FindFirstObjectByType<Inventory>();
        startingSpellCounter = gameManager.betaSpellCounter;
    }

    public void BetaLoop()
    {
        loopBarType = LoopBarType.SpellCombination;
        gameManager.ChangeSpellAllocation();
        OnCall();

        startingSpellCounter--;
        if(startingSpellCounter <= 0)
        {
            gameManager.betaMode = false;
        }
    }
    public void OnCall()
    {
        spellImage.sprite = gameManager.spellImage;
        spellArray = loopbarInventory.spellArray; //pointer for actual spell array
        UpdateSpellSprites();
        GetTypes();
        FindAnyObjectByType<UIManager>().spellbarAllocationAnimator.SetTrigger("BeginSpellAllocation");
    }
    private void UpdateSpellSprites()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (spellArray[i] == null)
            {
                
                inventorySlots[i].sprite = emptySlotSprite;
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
                        typeText[i].text = "Empty";
                        break;

                }   
            }
            
            else
            {
                typeText[i].text = "";
            }
        }
    }
    // Button Functions
    public void SlotOne()
    {
        SpellBarTypeCheck(0);
    }
    public void SlotTwo()
    {
        SpellBarTypeCheck(1);
    }
    public void SlotThree()
    {
        SpellBarTypeCheck(2);
    }
    public void SlotFour()
    {
        SpellBarTypeCheck(3);
    }
    public void SlotFive()
    {
        SpellBarTypeCheck(4);
    }
    public void SlotSix()
    {
        SpellBarTypeCheck(5);
    }
    public void SlotSeven()
    {
        SpellBarTypeCheck(6);
    }
    public void SlotEight()
    {
        SpellBarTypeCheck(7);
    }

    void SpellBarTypeCheck(int index)
    {
        switch (loopBarType)
        {
            case LoopBarType.SpellSwap:
                // Call Spell Swap Function
                SpellSwapEvaluator(index);
                break;
            case LoopBarType.SpellCombination:
                // Call Spell Combination Function
                SelectSpellReplacement(index);
                break;
            default:
                break;
        }
    }

    void SpellSwapEvaluator(int index)
    {
        
        if(spellToSwapOne == null && swapState == SwapState.None) // first click, spell isnt empty
        {
            spellToSwapOneIndex = index;
            spellToSwapOne = SpellCombinationNullEvaluator(index);
            swapState = SwapState.FirstSelected;
        }
        else if (spellToSwapTwo == null && swapState == SwapState.FirstSelected) // second click, spell isnt empty, swap happens
        {
            spellToSwapTwoIndex = index;
            spellToSwapTwo = SpellCombinationNullEvaluator(index);

            SwapSlots(spellToSwapOneIndex, spellToSwapTwoIndex);

            spellToSwapOne = null;
            spellToSwapTwo = null;
            swapState = SwapState.None;

            OnCall(); // Refreshes the UI to show the swapped spells
        }
    }

    Spell SpellCombinationNullEvaluator(int index) // SpellSwapEvaluator Wrapper
    {
        if(spellArray[index] == null)
        {
            return null;
        }
        else
        {
            return spellArray[index];
        }
    }

    public void SwapSlots(int indexOne, int indexTwo)
    {
        spellArray[indexOne] = spellToSwapTwo;
        spellArray[indexTwo] = spellToSwapOne;

        UpdateSpellSprites();
        loopbarInventory.UpdateSpellSprites(); // Refreshes the inventory's spell sprites to reflect the swapped spells
    }

    void SelectSpellReplacement(int index)
    {
        FindAnyObjectByType<AudioManager>().Play("UICONFIRM");


        bool isSingle;

        loopbarInventory.chosenSpell = GetChosenSpell();
        isSingle = loopbarInventory.CheckNewElementSelection(index);

        if (!isSingle)
        {
            loopbarInventory.UpdateSpellSprites();
        }


        if(gameManager.betaMode)
        {
            BetaLoop();
        }
        else
        {
            TransitionToGameplayMode(); 
        }
    }

    public void TransitionToGameplayMode()
    {
        TooltipManager._instance.HideTooltip();
        unityEvent.Invoke(); // Gameplay Mode
    }

    public Spell GetChosenSpell()
    {
        switch (gameManager.allocateSpell)
        {
            case Spell.SpellType.Fire:
                return spellReplacements[0];
            case Spell.SpellType.Water:
                return spellReplacements[1];
            case Spell.SpellType.Lightning:
                return spellReplacements[2];
            case Spell.SpellType.Dark:
                return spellReplacements[3];
            default:
                return null;
        }
    }
}
