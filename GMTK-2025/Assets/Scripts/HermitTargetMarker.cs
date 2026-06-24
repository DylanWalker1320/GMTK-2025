using UnityEngine;

public class HermitTargetMarket : MonoBehaviour
{
    void Update()
    {
        // Rotate and scale the target marker for visual flair
        transform.Rotate(0f, 0f, 90f * Time.deltaTime); // Rotate around Z axis
        transform.localScale = new Vector3(1f + Mathf.Sin(Time.time) * 0.1f, 1f + Mathf.Sin(Time.time) * 0.1f, 1f);
    }
}
