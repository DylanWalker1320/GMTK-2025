using UnityEngine;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;

[System.Serializable]
public class StatPanel
{
    public TextMeshProUGUI rarityText;
    public TextMeshProUGUI statDescription;
    public RawImage boxSprite;
}

public class LevelUpUI : MonoBehaviour
{
    private StatGenerator statGenerator;
    public StatPanel[] panels = new StatPanel[3];

    public void Awake()
    {
        statGenerator = FindAnyObjectByType<StatGenerator>();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            InitializeLevelUpUI();
        }
    }
    
    
    public void InitializeLevelUpUI()
    {
        for(int i = 0; i < 3; i++)
        {
            StatPanel panel = panels[i];
            string output = "";
            GeneratedStat statData = LevelUpStatsGenerator.GenerateStats("Default Stat");
            statGenerator.Initialize(statData, i, true);

            panel.boxSprite.color = DetermineColor(statData.statRarity);
            
            panel.rarityText.text = statGenerator.statData.statName; // stat name already set to rarity
            panel.rarityText.color = DetermineColor(statData.statRarity);

             foreach (var stat in statGenerator.statData.stats)
            {
                output += $"{stat}\n";
            }
            panel.statDescription.text = output;
        }
    }

    private Color DetermineColor(StatRarity rarity)
    {
        switch(rarity)
        {
            case StatRarity.Common:
                return Color.gray;
            case StatRarity.Uncommon:
                return Color.green;
            case StatRarity.Rare:
                return Color.blue;
            case StatRarity.Epic:
                return new Color(0.627451f, 0.1254902f, 0.9411765f); // purple
            case StatRarity.Legendary:
                return new Color(255, 165, 0); // orange
            default:
                return Color.white;
        }
    }

    public void SlotOne()
    {
        statGenerator.ApplyStats(0);
        // TODO Subtract Soul Cost
    }

    public void SlotTwo()
    {
        statGenerator.ApplyStats(1);
        // TODO Subtract Soul Cost        
    }

    public void SlotThree()
    {
        statGenerator.ApplyStats(0);
        // TODO Subtract Soul Cost        
    }


}
