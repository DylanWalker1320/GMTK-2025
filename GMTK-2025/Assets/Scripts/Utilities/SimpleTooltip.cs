using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleTooltip : Tooltip
{
    [SerializeField] private Vector2 tooltipOffset;

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
        TooltipManager._instance.SetAndShowTooltip(message);
    }

    public override void OnPointerExit(PointerEventData pointerEventData)
    {
        TooltipManager._instance.HideTooltip();
    }

    public override void OnSelect(BaseEventData baseEventData)
    {
        TooltipManager._instance.SetAndShowTooltip(message);
        TooltipManager._instance.UpdateTooltipPosition(transform.position + (Vector3)tooltipOffset);    
    }

    public override void OnDeselect(BaseEventData baseEventData)
    {
        TooltipManager._instance.HideTooltip();
    }
}
