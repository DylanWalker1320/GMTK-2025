using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


public class TooltipEnchantDescription : Tooltip
{
    [SerializeField] private InteractableLoopBar spellBarReference;
    public static UiMode uiMode = UiMode.SpellCombination;

    public enum UiMode
    {
        SpellCombination,
        Enchantment
    }

    public override void OnMouseDown(){}
    public override void OnMouseExit(){}
    public override void OnSelect(BaseEventData baseEventData){}
    public override void OnDeselect(BaseEventData baseEventData){}

    public override void OnPointerEnter(PointerEventData pointerEventData)
    {
        Debug.Log($"TooltipEnchantDescription: OnPointerEnter called. Current uiMode: {uiMode}");
        AssignMessage();

        TooltipManager._instance.SetAndShowTooltip(message);
    }

    public override void OnPointerExit(PointerEventData pointerEventData)
    {
        TooltipManager._instance.HideTooltip();
    }

    private void AssignMessage()
    {
        if (uiMode == UiMode.Enchantment)
        {
            message = EnchantmentDrop.enchantmentDescriptions[spellBarReference.GetEnchantmentType()];
        }
    }
}
