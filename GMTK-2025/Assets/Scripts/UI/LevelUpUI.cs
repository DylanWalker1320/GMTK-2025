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

public class LevelUpUI : MonoBehaviour
{
    private PlayerMovement player;
    private StatGenerator statGenerator;
    [SerializeField] private int rerollCost = 1;
    [SerializeField] private TextMeshProUGUI rerollText;
    public StatPanel[] panels = new StatPanel[3];
    public UnityEvent unityEvent;

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
    
    
    public void InitializeLevelUpUI()
    {
        for(int i = 0; i < 3; i++)
        {
            string output = "";
            StatPanel panel = panels[i];
            GeneratedStat statData = LevelUpStatsGenerator.GenerateStats("Default Stat");

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
        Time.timeScale = 1;
        TooltipManager._instance.HideTooltip();
        unityEvent.Invoke();
    }
    
    public void Reroll()
    {
        if(rerollCost > 0)
        {
            rerollCost -= 1;
            player.UpdateUI();
            InitializeLevelUpUI();
        }
    }

}
