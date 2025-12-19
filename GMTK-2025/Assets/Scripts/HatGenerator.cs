using UnityEngine;

public class HatGenerator : MonoBehaviour
{
    [SerializeField] private GameObject hatPrefab;
    [SerializeField] private GameObject hatContainer;
    [SerializeField] private Sprite[] hatSprites;
    [SerializeField] private GameObject player;
    private int numHats = 0;
    public bool debugMode = false;

    public void Start()
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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && debugMode)
        {
            StackHat();
        }   
    }

    public void StackHat()
    {
        GameObject hatInstance = GenerateHat();

        // Set the hatBelow reference for proper stacking
        PlayerHat hatScript = hatInstance.GetComponent<PlayerHat>();
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

    public GameObject GenerateHat()
    {
        GameObject hatInstance = Instantiate(hatPrefab);
        hatInstance.transform.SetParent(hatContainer.transform);

        // Reset local position and rotation
        hatInstance.transform.localPosition = Vector3.zero;
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

        // Generate and assign stats to the hat
        Hat hatScript = hatInstance.GetComponent<Hat>();
        if (hatScript != null)
        {
            // Generate stats using the static HatStatsGenerator
            GeneratedHat hatData = HatStatsGenerator.GenerateHatStats("Hat_" + numHats);
            
            // Pass the generated data to the Hat script
            hatScript.Initialize(hatData);
            
            // Debug output
            if (debugMode)
            {
                string output = "<color=#FFAA00>[Hat Generator]</color>\n";
                output += $"Generated Hat: {hatData.hatName} (Rarity: {hatData.rarity})\nStats:\n";
                foreach (var stat in hatData.stats)
                {
                    output += $"- {stat}\n";
                }

                Debug.Log(output);
            }
        }

        return hatInstance;
    }
}