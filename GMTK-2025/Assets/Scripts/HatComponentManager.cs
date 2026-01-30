
using UnityEngine;

public class HatComponentManager : MonoBehaviour
{
    public Hat hat;
    public SpriteRenderer front;
    public SpriteMask frontMask;
    public SpriteRenderer back;
    public SpriteRenderer outline;
    public SpriteRenderer pattern;
    public SpriteMask tipMask;
    public SpriteMask[] brimMasks;

    public void UpdateSpriteLayers()
    {

        int hatNumber = hat.hatNumber;

        // Update sorting orders based on hat number
        // Front: 1, Front Mask: 1-2, Back 0, Outline: 1, Pattern: 2, Tip Mask: 0-2, Brim Mask: 1-2, Add 3 for each stacked hat
        front.sortingOrder =                1 + (hatNumber * 3);
        frontMask.frontSortingOrder =       2 + (hatNumber * 3);
        frontMask.backSortingOrder =        1 + (hatNumber * 3);
        back.sortingOrder =                 0 + (hatNumber * 3);
        outline.sortingOrder =              1 + (hatNumber * 3);
        pattern.sortingOrder =              2 + (hatNumber * 3);
        tipMask.frontSortingOrder =         2 + (hatNumber * 3);
        tipMask.backSortingOrder =          0 + (hatNumber * 3);
        foreach (SpriteMask brimMask in brimMasks)
        {
            brimMask.frontSortingOrder =    3 + (hatNumber * 3);
            brimMask.backSortingOrder =     2 + (hatNumber * 3);
        }

        hat.hatSpriteMinLayer = back.sortingOrder;
    }

    public void ApplyComponents(HatComponents components)
    {
        pattern.sprite = components.pattern;
        front.color = components.color;
    }

    public void DisableShadow()
    {
        hat.hatShadow.SetActive(false);
    }
}
