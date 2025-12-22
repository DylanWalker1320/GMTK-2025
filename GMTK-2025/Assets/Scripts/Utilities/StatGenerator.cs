using UnityEngine;

public class StatGenerator : Stats
{
    public bool debugMode = false;

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.K) && debugMode)
        {
            GeneratedStat statData = LevelUpStatsGenerator.GenerateStats("Default Stat");
            Initialize(statData, true);
        }
    }
    public override void Initialize(GeneratedStat data, bool applyStats = true)
    {
        // Initialize by assigning the generated level up stats to statData
        statData = data;

        // Send string data to panels
        
        if (applyStats)
        {
                string output = "<color=#FFAA00>[Stat Generator]</color>\n";
                output += $"Generated Stat: {statData}";
                Debug.Log(output);
        }
    }
    private void ApplyStats()
    {
        

        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        // apply stats to palyer
        foreach (var stat in statData.stats)
        {
            switch (stat.type)
            {
                case LevelUpStatType.Speed:
                    player.maxSpeed += stat.value;
                    break;
                    
                case LevelUpStatType.Health:
                    player.health += stat.value;
                    player.maxHealth += stat.value;
                    break;
                    
                case LevelUpStatType.CastSpeed:
                    player.castSpeed += stat.value;
                    break;
                    
                case LevelUpStatType.CastStrength:
                    player.castStrength += stat.value;
                    break;
                    
                case LevelUpStatType.SpellLevel:
                    if (stat.LevelUpSpellLevelBonus != null)
                    {
                        // Upgrade the specific spell
                        for (int i = 0; i < stat.LevelUpSpellLevelBonus.levelBonus; i++)
                        {
                            Spell.UpgradeSpell(stat.LevelUpSpellLevelBonus.spell);
                        }
                    }
                    break;
            }
        }
        player.UpdateUI();
    }
}
