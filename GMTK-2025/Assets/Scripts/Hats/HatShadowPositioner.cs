using System.Reflection.Emit;
using UnityEngine;

public class HatShadowPositioner : MonoBehaviour
{
    public Hat Hat;
    private float hatHeight;
    private static float shadowAngle = 75f; // Angle of the light source in degrees (counter-clockwise from horizontal)
    private float baseOffset = 1f; // Base offset to ensure shadow is not directly under the hat
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        if (Hat == null)
        {
            Hat = GetComponentInParent<Hat>();
        }

        // Find hat height
        hatHeight = Hat.initialYOffset + (Hat.hatNumber * Hat.yDifference);

        // Calculate shadow position as an offset found with x = h / tan(shadowAngle)
        float shadowOffsetX = hatHeight / Mathf.Tan(shadowAngle * Mathf.Deg2Rad) + baseOffset; 

        // Position the shadow
        transform.localPosition = new Vector3(shadowOffsetX, 0, 0);

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Adjust the sorting order to be below the hat
            spriteRenderer.sortingOrder = Hat.hatSpriteMinLayer - 1;
        }
    }
}
