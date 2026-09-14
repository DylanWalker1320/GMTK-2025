using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class StatPanel
{
    public int soulCost;
    public Button button;
    public TextMeshProUGUI rarityText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI statDescription;
    public RawImage boxSprite;
}

public class LevelUpUI : MonoBehaviour // Changed to StatShopUI
{
    private PlayerMovement player;
    private StatGenerator statGenerator;
    [SerializeField] private int rerollCost = 1;
    [SerializeField] private TextMeshProUGUI rerollText;
    public StatPanel[] panels = new StatPanel[3];

    public void Awake()
    {
        player = FindAnyObjectByType<PlayerMovement>();
        statGenerator = FindAnyObjectByType<StatGenerator>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Exit();
        }
    }
    
    
    public void InitializeStatShopUI()
    {
        for(int i = 0; i < 3; i++)
        {
            string output = "";
            StatPanel panel = panels[i];
            
            GeneratedStat statData = LevelUpStatsGenerator.GenerateStats("Default Stat");

            // List<HatStat> stats = new List<HatStat>();
            // stats = HatStatsGenerator.GenerateHatStats(HatStatsGenerator.GenerateRarity(), 1);

            statGenerator.Initialize(statData, i, true);
            
            rerollText.text = rerollCost.ToString();
            panels[i].button.interactable = true;
            panel.soulCost = LevelUpStatDefinitions.StatSoulCost[statData.statRarity];
            panel.costText.text = panel.soulCost.ToString();
            panel.rarityText.text = statGenerator.statData.statName; // stat name already set to rarity
            panel.boxSprite.color = DetermineColor(statData.statRarity);
            panel.rarityText.color = DetermineColor(statData.statRarity);

             foreach (var stat in statGenerator.statData.stats)
            {
                output += $"{stat}\n";
            }
            panel.statDescription.text = output;
        }
    }

    // public void ApplyStats(int index)
    // {
    //     PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
    //     // apply stats to player
    //     foreach (var stat in panelStats[index].stats)
    //     {
    //         switch (stat.type)
    //         {
    //             case LevelUpStatType.Speed:
    //                 player.maxSpeed += stat.value;
    //                 if (debugMode) Debug.Log($"Speed increased from {player.maxSpeed - stat.value} to {player.maxSpeed}");
    //                 break;
                    
    //             case LevelUpStatType.Health:
    //                 player.health += stat.value;
    //                 player.maxHealth += stat.value;
    //                 if (debugMode) Debug.Log($"Max Health increased from {player.maxHealth - stat.value} to {player.maxHealth}");
    //                 break;

    //             case LevelUpStatType.DashStrength:
    //                 player.dashStrength += stat.value;
    //                 if (debugMode) Debug.Log($"Dash Strength increased from {player.dashStrength - stat.value} to {player.dashStrength}");
    //                 break;
                    
    //             case LevelUpStatType.CastSpeed:
    //                 player.castSpeed += stat.value;
    //                 if (debugMode) Debug.Log($"Cast Speed increased from {player.castSpeed - stat.value} to {player.castSpeed}");
    //                 break;
                    
    //             case LevelUpStatType.CastStrength:
    //                 player.castStrength += stat.value;
    //                 if (debugMode) Debug.Log($"Cast Strength increased from {player.castStrength- stat.value} to {player.castStrength}");
    //                 break;
                    
    //             case LevelUpStatType.SpellLevel:
    //                 if (stat.LevelUpSpellLevelBonus != null)
    //                 {
    //                     // Upgrade the specific spell
    //                     for (int i = 0; i < stat.LevelUpSpellLevelBonus.levelBonus; i++)
    //                     {
    //                         Spell.UpgradeSpell(stat.LevelUpSpellLevelBonus.spell);
    //                     }
    //                     if (debugMode) Debug.Log($"Spell Bonus of {stat.LevelUpSpellLevelBonus.spell} increased by {stat.LevelUpSpellLevelBonus}");
    //                 }
    //                 break;
    //         }
    //     }
    //     FindAnyObjectByType<UIManager>().updateStatTrackerUI();
    //     player.UpdateUI();
    // }

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
                return Color.magenta; // purple
            case StatRarity.Legendary:
                return Color.red; // orange
            default:
                return Color.white;
        }
    }

    private void TurnButtonOff(int index)
    {
        panels[index].button.interactable = false;
        panels[index].rarityText.text = "Purchased";
        panels[index].statDescription.text = string.Empty;
    }

    public void SlotOne()
    {
        if(player.souls - panels[0].soulCost >= 0)
        {
            TurnButtonOff(0);
            player.souls -= panels[0].soulCost;
            statGenerator.ApplyStats(0);
            Statue.TogglePurchaseAvailability(true);
            Exit();
            
        }
        
    }

    public void SlotTwo()
    {
        if(player.souls - panels[1].soulCost >= 0)
        {
            TurnButtonOff(1);
            player.souls -= panels[1].soulCost;
            statGenerator.ApplyStats(1);
            Statue.TogglePurchaseAvailability(true);
            Exit();
        }    
    }

    public void SlotThree()
    {
        if(player.souls - panels[2].soulCost >= 0)
        {
            TurnButtonOff(2);
            player.souls -= panels[2].soulCost;
            statGenerator.ApplyStats(2);
            Statue.TogglePurchaseAvailability(true);
            Exit();
        }     
    }

    public void Exit()
    {
        TooltipManager._instance.HideTooltip();
        FindAnyObjectByType<UIManager>().SetActiveStatShopUI("ExitStatShop");
    }
    
    public void Reroll()
    {
        if(rerollCost > 0)
        {
            FindAnyObjectByType<UIManager>().SetActiveStatShopUI("RerollStats");
            rerollCost -= 1;
            player.UpdateUI();
            InitializeStatShopUI();
        }
    }

}
