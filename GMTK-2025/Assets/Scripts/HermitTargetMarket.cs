using UnityEngine;

public class HermitTargetMarker : MonoBehaviour
{
    void Update()
    {
        // Rotate and scale the target marker for visual flair
        transform.Rotate(0f, 0f, 90f * Time.deltaTime); // Rotate around Z axis
    }
}
