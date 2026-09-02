using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public string message;


    public abstract void OnMouseDown();

    public abstract void OnMouseExit();

    public abstract void OnPointerEnter(PointerEventData pointerEventData);

    public abstract void OnPointerExit(PointerEventData pointerEventData);

    public abstract void OnSelect(BaseEventData baseEventData);

    public abstract void OnDeselect(BaseEventData baseEventData);
}
