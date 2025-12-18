using UnityEngine;
using System.Collections;

public class Hat : MonoBehaviour
{
    public GameObject hatBelow;
    private static readonly float moveThreshold = 0.01f;

    void Update ()
    {
        // If this is the first hat, rigidly position it above the player.
        if (hatBelow.CompareTag("Player"))
        {
            transform.position = hatBelow.transform.position + new Vector3(0, 1.5f, 0); 
        }

        // Otherwise, try and lerp to the hat below it.
        if (Vector3.Distance(transform.position, hatBelow.transform.position + new Vector3(0, 0.25f, 0)) > moveThreshold)
        {
            // Clamp the y position to be above the hat below
            transform.position = new Vector3(transform.position.x, hatBelow.transform.position.y + 0.25f, transform.position.z);
            StartCoroutine(MoveHat());
        }
    }

    IEnumerator MoveHat()
    {
        if (hatBelow != null)
        {
            Vector3 targetPosition = hatBelow.transform.position + new Vector3(0, 0.25f, 0);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
        }
        yield return null;
    }   
}
