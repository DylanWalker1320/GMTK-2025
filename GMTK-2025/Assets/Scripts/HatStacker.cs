using System.Runtime.Serialization;
using UnityEngine;

public class HatStacker : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject hatPrefab;
    [SerializeField] private GameObject hatContainer;
    [SerializeField] private Sprite[] hatSprites;
    private int numHats = 0;
    public bool debugMode = false;

    public void StackHat()
    {
        if (hatPrefab == null)
        {
            Debug.LogError("Hat prefab is not assigned.");
            return;
        }

        if (hatContainer == null)
        {
            Debug.LogError("Hat container is not assigned.");
            return;
        }

        if (hatSprites.Length == 0)
        {
            Debug.LogError("No hat sprites available.");
            return;
        }

        GameObject hatInstance = Instantiate(hatPrefab);
        hatInstance.transform.SetParent(hatContainer.transform);

        // Reset local position and rotation
        hatInstance.transform.localPosition = player.transform.InverseTransformPoint(new Vector3(0, 0.25f * numHats, 0));
        hatInstance.transform.localRotation = Quaternion.identity;
        numHats++;

        hatInstance.name = "Hat_" + numHats;

        // Assign a random sprite to the hat
        SpriteRenderer hatSpriteRenderer = hatInstance.GetComponent<SpriteRenderer>();
        if (hatSpriteRenderer != null)
        {
            int randomIndex = Random.Range(0, hatSprites.Length);
            hatSpriteRenderer.sprite = hatSprites[randomIndex];
        }
        else
        {
            Debug.LogError("Hat prefab does not have a SpriteRenderer component.");
        }

        // Set the hatBelow reference for proper stacking
        Hat hatScript = hatInstance.GetComponent<Hat>();
        if (hatScript != null)
        {
            if (numHats == 1)
            {
                hatScript.hatBelow = player; // First hat sits on the player
            }
            else
            {
                Transform previousHatTransform = hatContainer.transform.GetChild(numHats - 2);
                hatScript.hatBelow = previousHatTransform.gameObject; // Subsequent hats sit on the previous hat
            }
        }
        else
        {
            Debug.LogError("Hat prefab does not have a Hat script component.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && debugMode)
        {
            StackHat();
        }   
    }
}
