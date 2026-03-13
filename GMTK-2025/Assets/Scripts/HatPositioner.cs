using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HatPositioner : MonoBehaviour
{
    public List<Vector2> hatPositions;
    public static Vector2 currentOffset;   
    public int animFrame = 0;
    public SpriteRenderer spriteRenderer;

    void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
    
    void Update()
    {
        // Get the last character of the sprite name as the animation frame. Terrible, awful code but animation keyframes have forced my hand...
        animFrame = (int)char.GetNumericValue(spriteRenderer.sprite.name[ spriteRenderer.sprite.name.Length - 1 ]);

        currentOffset = hatPositions[animFrame];
    }
}