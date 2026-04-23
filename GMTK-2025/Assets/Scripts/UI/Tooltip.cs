using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string message;


    public abstract void OnMouseDown();

    public abstract void OnMouseExit();

    public abstract void OnPointerEnter(PointerEventData pointerEventData);

    public abstract void OnPointerExit(PointerEventData pointerEventData);
}
