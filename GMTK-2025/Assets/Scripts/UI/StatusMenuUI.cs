using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class StatusMenuUI : MonoBehaviour
{
    private PlayerMovement playerStats;
    [Header("Hats")]
    [SerializeField] private List<HatNodeUI> hatNodes = new(); // List of UI nodes -> HatNodeUI type makes it easier

    [SerializeField] private List<Hat> sortedHats; // Sorted List to cast data onto refreshed UI nodes
    [SerializeField] private HatGenerator playerHats;
    [Header("UI Elements")]
    [SerializeField] private Transform gridContainer;
    [SerializeField] private HatNodeUI hatNodePrefab;
    [Header("Text Elements")]
    [SerializeField] private TextMeshProUGUI playerStatusText;
    [SerializeField] private TextMeshProUGUI playerAttributesText;
    [Header("Base Player Stats")]
    [SerializeField] private const float baseSpeed = 5;
    [SerializeField] private const float baseCastSpeed = 1;
    [SerializeField] private const float baseCastStrength = 1;
    [SerializeField] private const float baseDashStrength = 20;
    [SerializeField] private const float baseDashCooldown = 1;
    [SerializeField] private const float baseXpPullRange = 5;

    void Awake()
    {
        playerStats = FindAnyObjectByType<PlayerMovement>();
    }
    public void OnCall()
    {
        UpdateAllDisplays();
    }

    void UpdateAllDisplays()
    {
        UpdatePlayerStats();
        RefreshHatList();
    }

    void UpdatePlayerStats() // Will show some stats through percentages TODO: add dash cooldown
    {
        playerStatusText.text = "Lvl: " + playerStats.level + "\n" + "HP: " + playerStats.health + "/" + playerStats.maxHealth + "\n" + "EXP until next Lvl: " + (playerStats.nextLevelExperience - playerStats.experience);
        playerAttributesText.text = "SPD: +" + ConvertToPercentage(playerStats.maxSpeed, baseSpeed) + "%" + "\n" + "Cast STR: +" + ConvertToPercentage(playerStats.castStrength, baseCastSpeed) + "%" 
            + "\n" + "Cast SPD: +" + ConvertToPercentage(playerStats.castSpeed, baseCastSpeed) + "%" + "\n" + "Dash STR: +" + ConvertToPercentage(playerStats.dashStrength, baseDashStrength) + "%"
                + "\n" + "XP Pull: +" + ConvertToPercentage(playerStats.xpParticleSystem.endRange, baseXpPullRange) + "%";


    }

    void RefreshHatList()
    {
        if(playerHats.stackedHatObjects.Count == 0) // No hats acquired
        {
            return;
        }
        else if(playerHats.stackedHatObjects.Count == 1 && IsRefreshRequired()) // 1 hat acquired
        {
            GenerateNewNode(0);
            SetNodeData(0);
        }
        else if (IsRefreshRequired())
        {
            GenerateNewNodes(); // Generate extra nodes
            SortHatList(); // Sort player hats based on rarity + stat score

            // Cast new order onto current UI
            for(int i = 0; i < hatNodes.Count(); i++)
            {
                SetNodeData(i); // Set each node's information from top to bottom using the sorted hat list data
            }
        }
    }

    private void SortHatList()
    {

        sortedHats.Sort((a, b) =>
        {
            int rarityComparison = b.hatData.rarity.CompareTo(a.hatData.rarity);

            if (rarityComparison != 0)
            {
                return rarityComparison;
            }

            return b.hatData.GetHatScore().CompareTo(a.hatData.GetHatScore());

        });

        // for (int i = 0; i < sortedHats.Count; i++)
        // {
        //     Debug.Log(
        //         $"{i}: {sortedHats[i].hatData.hatName} | " +
        //         $"{sortedHats[i].hatData.rarity} | "
        //     );
        // }
        // for (int i = 0; i < hatNodes.Count; i++)
        // {
        //     Debug.Log(
        //         $"{i}: {sortedHats[i].hatData.hatName} | " +
        //         $"{sortedHats[i].hatData.rarity} | "
        //     );
        // }
    }

    private void GenerateNewNodes()
    {
        // Instantiate new nodes until they match player hat count
        for(int i = hatNodes.Count(); i < playerHats.stackedHatObjects.Count; i++) 
        {
            GenerateNewNode(i);
        }
    }
    
    private void GenerateNewNode(int index)
    {
        HatNodeUI newNode = Instantiate(hatNodePrefab, gridContainer);
        Hat newHat = playerHats.stackedHatObjects[index].GetComponent<Hat>();

        sortedHats.Add(newHat); // add for new sort
        hatNodes.Add(newNode);
    }

    private void SetNodeData(int index)
    {
        hatNodes[index].nodeData = sortedHats[index].hatData;
        hatNodes[index].hatVisuals = sortedHats[index].GetHatComponent();
        hatNodes[index].SetHat();
    }

    private bool IsRefreshRequired()
    {
        if (hatNodes.Count == playerHats.stackedHatObjects.Count) // No change no extra work
        {
            return false;
        }
        else // new player hats acquired that UI has not accounted for yet
        {
            return true;
        }
    }

    float ConvertToPercentage(float currStat, float baseStat)
    {
        return (currStat / baseStat - 1) * 100;
    }
}
