using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class StatusMenuUI : MonoBehaviour
{
    private PlayerMovement playerStats;
    private readonly List<HatNodeUI> hatNodes = new();
    [Header("Hats")]
    [SerializeField] private HatGenerator playerHats;
    [SerializeField] private List<Hat> hats;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStats = FindAnyObjectByType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
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

    void UpdatePlayerStats() // Will show some stats through percentages
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
        else if(playerHats.stackedHatObjects.Count == 1) // 1 hat acquired
        {
            GenerateNodes();
            hatNodes[0].SetHat();
        }
        else if (IsRefreshRequired())
        {
            SortHatList(); // Sort list if there are new hats
            GenerateNodes(); // Generate Hat UI Nodes

            for(int i = 0; i < hatNodes.Count; i++)
            {
                hatNodes[i].SetHat(); // Set up node details
            }
        }
    }

    private void SortHatList()
    {

        hatNodes.Sort((a, b) =>
        {
            int rarityComparison = b.nodeData.rarity.CompareTo(a.nodeData.rarity);

            if (rarityComparison != 0)
            {
                return rarityComparison;
            }

            return b.nodeData.GetHatScore().CompareTo(a.nodeData.GetHatScore());

        });
    }

    private void GenerateNodes()
    {
        for(int i = hatNodes.Count; i < playerHats.stackedHatObjects.Count; i++) 
        {
            HatNodeUI newNode = Instantiate(hatNodePrefab, gridContainer);
            Hat currHatData = playerHats.stackedHatObjects[i].GetComponent<Hat>();

            newNode.nodeData = currHatData.hatData;
            newNode.nodeVisuals = currHatData.hatVisuals;

            hatNodes.Add(newNode);
        }
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
