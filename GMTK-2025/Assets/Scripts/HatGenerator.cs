using System.Collections.Generic;
using UnityEngine;

public class HatGenerator : MonoBehaviour
{
    [SerializeField] private GameObject hatContainer;
    [SerializeField] public GameObject[] hatTypes;
    [SerializeField] public Sprite[] hatPatterns;
    [SerializeField] private GameObject player;
    private int numHatsStacked = 0;
    public bool debugMode = false;
    public List<GameObject> stackedHatObjects = new List<GameObject>();

    public void Start()
    {
        if (hatTypes == null || hatTypes.Length == 0)
        {
            Debug.LogError("No hat types available.");
            return;
        }

        if (hatContainer == null)
        {
            Debug.LogError("Hat container is not assigned.");
            return;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && debugMode)
        {
            StackHat(GenerateHat());
        }
    }
    

    public void StackHat(GameObject hatInstance)
    {
        
        hatInstance.transform.SetParent(hatContainer.transform);
        stackedHatObjects.Add(hatInstance);
        
        // Set the hatBelow reference for proper stacking
        Hat hatScript = hatInstance.GetComponent<Hat>();
        if (hatScript != null)
        {
            if (numHatsStacked == 0)
            {
                hatScript.SetHatBelow(player); // First hat sits on the player
            }
            else
            {
                hatScript.SetHatBelow(stackedHatObjects[numHatsStacked - 1]); // Subsequent hats sit on the previous hat
            }

            hatScript.hatNumber = numHatsStacked;
            numHatsStacked++;
        }
        else
        {
            Debug.LogError("Hat prefab does not have a Hat script component.");
        }
        hatScript.InitializeNewHat();
    }

    public void StackHatWithStats(GeneratedHat hatData) // Function for loading hats with predefined stats
    {
        GameObject hatInstance = GenerateHatWithStats(hatData);

        // Set the hatBelow reference for proper stacking
        Hat hatScript = hatInstance.GetComponent<Hat>();
        if (hatScript != null)
        {
            if (numHatsStacked == 0)
            {
                hatScript.SetHatBelow(player); // First hat sits on the player
            }
            else
            {
                hatScript.SetHatBelow(stackedHatObjects[numHatsStacked - 1]); // Subsequent hats sit on the previous hat
            }

            hatScript.hatNumber = numHatsStacked;
            numHatsStacked++;
        }
        else
        {
            Debug.LogError("Hat prefab does not have a Hat script component.");
        }

        // SpriteRenderer sr = hatInstance.GetComponent<SpriteRenderer>();
        // sr.sortingOrder = numHatsStacked + 2; // Ensure proper rendering order
    }

    public GameObject GenerateHat(bool applyStats = false)
    {
        GeneratedHat hatData = HatStatsGenerator.GenerateHatStats(hatName: "Default Hat");
        
        // Generate components
        hatData.components = GetRandomHatComponents(hatData);
        GameObject hatInstance = Instantiate(hatData.components.hatType);

        // Reset local position and rotation
        hatInstance.transform.localPosition = Vector3.zero;
        hatInstance.transform.localRotation = Quaternion.identity;

        // Generate and assign stats to the hat
        Hat hatScript = hatInstance.GetComponent<Hat>();
        if (hatScript != null)
        {
            // Pass the generated data to the Hat script
            hatScript.Initialize(hatData, applyStats);
            
            hatScript.hatData.hatName = GenerateHatName(hatData.rarity);
            hatInstance.name = hatScript.hatData.hatName;

            // Debug output
            if (debugMode)
            {
                string output = "<color=#FFAA00>[Hat Generator]</color>\n";
                output += $"Generated Hat: {hatData}";
                Debug.Log(output);
            }
        }
        return hatInstance;
    }

    public GameObject GenerateHatWithStats(GeneratedHat hatData, bool applyStats = true) // Wrapper for loading hats with predefined stats
    {
        GameObject hatInstance = Instantiate(hatData.components.hatType);
        hatInstance.transform.SetParent(hatContainer.transform);
        stackedHatObjects.Add(hatInstance);
        hatInstance.name = hatData.hatName;

        // Assign the provided sprite if available
        //SpriteRenderer sr = hatInstance.GetComponent<SpriteRenderer>();
        //if (sr != null && hatData.hatSprite != null) sr.sprite = hatData.hatSprite;

        // Reset local position and rotation
        hatInstance.transform.localPosition = Vector3.zero;
        hatInstance.transform.localRotation = Quaternion.identity;

        // Assign provided stats to the hat
        Hat hatScript = hatInstance.GetComponent<Hat>();
        if (hatScript != null)
        {
            // Pass the provided data to the Hat script
            hatScript.Initialize(hatData, applyStats);
        }

        // Debug output
        if (debugMode)
        {
            string output = "<color=#FFAA00>[Hat Generator]</color>\n";
            output += $"Loaded Hat: {hatData}";
            Debug.Log(output);
        }

        return hatInstance;
    }

    private HatComponents GetRandomHatComponents(GeneratedHat hatData)
    {
        HatComponents components = new HatComponents();
        components.hatType = hatTypes[Random.Range(0, hatTypes.Length)];
        components.pattern = hatPatterns[Random.Range(0, hatPatterns.Length)];
    
        components.color = hatData.rarity switch
        {   //                                   minHue     maxHue minSat maxSat minVal maxVal
            Rarity.Common =>    Random.ColorHSV(0f,        0f,        0.5f, 0f, 0f,    1f), // Hue: 0 - 0, saturation: 0% to 0%, Value: 50% to 100%        -> Shades of grey
            Rarity.Uncommon =>  Random.ColorHSV(90f/360f,  150f/360f, 0.5f, 1f, 0.75f, 1f), // Hue: 90 - 150, Saturation: 50% to 100%, Value: 75% to 100%  -> Shades of green 
            Rarity.Rare =>      Random.ColorHSV(200f/360f, 260f/360f, 0.5f, 1f, 0.75f, 1f), // Hue: 200 - 260, Saturation: 50% to 100%, Value: 75% to 100% -> Shades of blue
            Rarity.Epic =>      Random.ColorHSV(280f/360f, 310f/360f, 0.5f, 1f, 0.75f, 1f), // Hue: 280 - 310, Saturation: 50% to 100%, Value: 75% to 100% -> Shades of purple - fuscia
            Rarity.Legendary => Random.ColorHSV(30f/360f,  60f/360f,  0.5f, 1f, 0.75f, 1f), // Hue: 20 - 40, Saturation: 50% to 100%, Value: 75% to 100%   -> Shades of orange - yellow
            _ => Color.white
        };

        return components;
    }

    public void ClearHats()
    {
        for (int i = stackedHatObjects.Count - 1; i >= 0; i--)
        {
            Destroy(stackedHatObjects[i]);
        }
        stackedHatObjects.Clear();
        numHatsStacked = 0;
    }

    private string GenerateHatName(Rarity rarity)
    {

        List<string> commonPrefixes = new List<string> { "Simple", "Crude", "Shabby", "Faded", "Plain", "Worn", "Sturdy", "Dusty" };
        List<string> commonTypes = new List<string> { "Cap", "Toque", "Hood", "Bandana", "Beanie" };
        List<string> commonSuffixes = new List<string> { "of the Novice", "of Beginnings", "of the Commoner" };

        List<string> uncommonPrefixes = new List<string> { "Polished", "Infused", "Keen", "Balanced", "Fancy", "Brisk", "Touched" };
        List<string> uncommonTypes = new List<string> { "Pointed Hat", "Circlet", "Cowl", "Wide-brim", "Ushanka" };
        List<string> uncommonSuffixes = new List<string> { "of Insight", "of the Apprentice", "of Focus", "of the Winds" };

        List<string> rarePrefixes = new List<string> { "Arcane", "Pristine", "Shimmering", "Gilded", "Volatile", "Frost-kissed", "Embered" };
        List<string> rareTypes = new List<string> { "Tiara", "Crown", "Wizard's Helm", "Mitre", "Conical Hat" };
        List<string> rareSuffixes = new List<string> { "of the Elements", "of Shadow", "of the Adept", "of Respite" };

        List<string> epicPrefixes = new List<string> { "Mythical", "Ethereal", "Celestial", "Dragonhide", "Runed", "Spectral", "Stormforged" };
        List<string> epicTypes = new List<string> { "Diadem", "Great-Hat", "Arch-Hood", "Sorcerer's Crest" };
        List<string> epicSuffixes = new List<string> { "of the Void", "of Dragon-Fire", "of the Unspoken", "of Eternity" };

        List<string> legendaryPrefixes = new List<string> { "Divine", "Mythical", "Multiversal", "Spicy", "Sovereign", "Cosmic", "Primordial" };
        List<string> legendaryTypes = new List<string> { "Halo", "Apex-Crown", "Zenith-Cap", "World-Brim" };
        List<string> legendarySuffixes = new List<string> { "of the Cosmos", "of Infinite Wisdom", "of the Eternal Flame", "of the End" };

        switch (rarity)
        {
            case Rarity.Common:
                return $"{commonPrefixes[Random.Range(0, commonPrefixes.Count)]} {commonTypes[Random.Range(0, commonTypes.Count)]} {commonSuffixes[Random.Range(0, commonSuffixes.Count)]}";
            case Rarity.Uncommon:
                return $"{uncommonPrefixes[Random.Range(0, uncommonPrefixes.Count)]} {uncommonTypes[Random.Range(0, uncommonTypes.Count)]} {uncommonSuffixes[Random.Range(0, uncommonSuffixes.Count)]}";
            case Rarity.Rare:
                return $"{rarePrefixes[Random.Range(0, rarePrefixes.Count)]} {rareTypes[Random.Range(0, rareTypes.Count)]} {rareSuffixes[Random.Range(0, rareSuffixes.Count)]}";
            case Rarity.Epic:
                return $"{epicPrefixes[Random.Range(0, epicPrefixes.Count)]} {epicTypes[Random.Range(0, epicTypes.Count)]} {epicSuffixes[Random.Range(0, epicSuffixes.Count)]}";
            case Rarity.Legendary:
                return $"{legendaryPrefixes[Random.Range(0, legendaryPrefixes.Count)]} {legendaryTypes[Random.Range(0, legendaryTypes.Count)]} {legendarySuffixes[Random.Range(0, legendarySuffixes.Count)]}";
            default:
                return "Unknown Hat";
        }
    }
}