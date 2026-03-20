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
    [SerializeField] private TextMeshProUGUI[] typeText = new TextMeshProUGUI[8];
    [SerializeField] private Image spellImage;
    [SerializeField] private Spell[] spellReplacements = new Spell[4];

    [SerializeField] private UnityEvent unityEvent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        loopbarInventory = FindFirstObjectByType<Inventory>();
        startingSpellCounter = gameManager.betaSpellCounter;
    }
    public void BetaLoop()
    {
        gameManager.ChangeSpellAllocation();
        OnCall();

        FindAnyObjectByType<UIManager>().spellbarAllocationAnimator.SetTrigger("BeginSpellAllocation");

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
        GetSpellSprites();
        GetTypes();
    }
    private void GetSpellSprites()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (spellArray[i] == null)
            {
                //Debug.Log("Spell is null, skipping check.");
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
                            typeText[i].text = "Empty";
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
        bool isSingle;

        loopbarInventory.chosenSpell = GetChosenSpell();
        isSingle = loopbarInventory.CheckNewElementSelection(index);

        if (!isSingle)
        {
            GetSpellSprites();
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
