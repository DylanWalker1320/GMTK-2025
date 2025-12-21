using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HatPositioner : MonoBehaviour
{
    public List<Vector2> hatPositions = new List<Vector2> { new Vector2(1,0), new Vector2(0,-1), new Vector2(-1,0), new Vector2(0,1) };
    public int currentHatIndex = 0;
    public bool isWalking = false;
    public Transform hatContainer;
    public float positionLerpSpeed = 10f;
    void UpdatePosition()
    {
        if (isWalking)
        {
            currentHatIndex = (currentHatIndex + 1) % hatPositions.Count;
            StartCoroutine(MoveHats());
        } else
        {
            currentHatIndex = 0;
            StartCoroutine(MoveHats());
        }
    }

    IEnumerator MoveHats()
    {
        // Lerp the hat container to the target position
        if (hatContainer != null)
        {
            Vector2 targetPos = hatPositions[currentHatIndex];
            hatContainer.localPosition = Vector3.Lerp(hatContainer.localPosition, new Vector3(targetPos.x, targetPos.y, 0), Time.deltaTime * positionLerpSpeed);
        }

        yield return null;
    }
}