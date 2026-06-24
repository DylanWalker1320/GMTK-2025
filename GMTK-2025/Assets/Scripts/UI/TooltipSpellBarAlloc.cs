using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipSpellBarAlloc : Tooltip
{
    [SerializeField] private InteractableLoopBar spellBarReference;
    [SerializeField] private Inventory spellCombinationsReference;
    [SerializeField] private int spellBoxIndex;
    private string spellBoxMarkupText;
    private string chosenSpellMarkupText;
    private bool emptyBox;
    public override void OnMouseDown()
    {
        TooltipManager._instance.SetAndShowTooltip(message);
    }

    public override void OnMouseExit()
    {
        TooltipManager._instance.HideTooltip();
    }

    public override void OnPointerEnter(PointerEventData pointerEventData)
    {

        SetElementalTextColours();
        AssignMessage();

        TooltipManager._instance.SetAndShowTooltip(message);
    }

    public override void OnPointerExit(PointerEventData pointerEventData)
    {
        TooltipManager._instance.HideTooltip();
    }

    private void SetElementalTextColours()
    {
        // Sets first two markup text colors based on spell types
        if(spellBarReference.spellArray[spellBoxIndex] == null)
        {
            emptyBox = true;
            spellBoxMarkupText = "white";
        }
        else
        {
            emptyBox = false;
            spellBoxMarkupText = SpellTypeToColor(spellBarReference.spellArray[spellBoxIndex].spellType1);
        }
        chosenSpellMarkupText = SpellTypeToColor(spellBarReference.GetChosenSpell().spellType1);
    }

    private string SpellTypeToColor(Spell.SpellType type)
    {
        switch (type)
        {
            case Spell.SpellType.Fire:
                return "red";
            case Spell.SpellType.Water:
                return "blue";
            case Spell.SpellType.Lightning:
                return "yellow";
            case Spell.SpellType.Dark:
                return "purple";
            default:
                return "white";
        }
    }

    private void AssignMessage()
    {
        // if no combination, output chosen spell text as final result
        if(emptyBox)
        {
            message = $"<color={spellBoxMarkupText}>{GetSpellBoxName()}</color> + <color={chosenSpellMarkupText}>{spellBarReference.GetChosenSpell().spellType1}</color>\n-> <color={chosenSpellMarkupText}>{spellBarReference.GetChosenSpell().spellType1}</color>";            
        }
        else
        {
            if (spellBarReference == null)
            {
                Debug.Log("TooltipSpellBarAlloc: spellBarReference is null.");
                return;
            }

            Spell chosenSpell = spellBarReference.GetChosenSpell();
            if (chosenSpell == null)
            {
                Debug.Log("TooltipSpellBarAlloc: chosenSpell is null.");
                return;
            }

            Spell selectedSpell = spellBarReference.spellArray[spellBoxIndex];
            if (selectedSpell == null)
            {
                Debug.Log("TooltipSpellBarAlloc: selectedSpell is null.");
                return;
            }

            Spell finalSpellCombination = FindFinalSpellCombinationText();
            if (finalSpellCombination == null)
            {
                Debug.Log("TooltipSpellBarAlloc: finalSpellCombination is null.");
                return;
            }

            message = $"<color={spellBoxMarkupText}>{GetSpellBoxName()}</color> + <color={chosenSpellMarkupText}>{chosenSpell.spellType1}</color>\n-> <color={finalSpellCombination.markupColor}>{finalSpellCombination.name}</color>";   
        }        
    }

    private Spell FindFinalSpellCombinationText()
    {
        return spellCombinationsReference.spellCombinations.OutputSpellCombination(spellBarReference.GetChosenSpell(), spellBarReference.spellArray[spellBoxIndex]);
    }

    private string GetSpellBoxName()
    {
        if(!emptyBox)
        {
            return spellBarReference.spellArray[spellBoxIndex].spellType1.ToString();
        }
        else
        {
            return "Empty";
        }
    }
}
