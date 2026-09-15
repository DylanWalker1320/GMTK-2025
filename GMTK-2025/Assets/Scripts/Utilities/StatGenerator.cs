using UnityEngine;

public class StatGenerator : Stats
{
    public bool debugMode = false;
    public GeneratedStat[] panelStats = new GeneratedStat[3];

    public override void Initialize(GeneratedStat data, int index, bool applyStats = true)
    {
        // Initialize by assigning the generated level up stats to statData
        statData = data;
        panelStats[index] = statData;

        // Send string data to panels
        
        if (applyStats && debugMode)
        {
                string output = "<color=#FFAA00>[Stat Generator]</color>\n";
                output += $"Generated Stat: {statData}";
                Debug.Log(output);
        }


    }
    public void ApplyStats(int index)
    {
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        // apply stats to player
        foreach (var stat in panelStats[index].stats)
        {
            switch (stat.type)
            {
                case LevelUpStatType.Speed:
                    player.maxSpeed += stat.value;
                    if (debugMode) Debug.Log($"Speed increased from {player.maxSpeed - stat.value} to {player.maxSpeed}");
                    break;
                    
                case LevelUpStatType.Health:
                    player.health += stat.value;
                    player.maxHealth += stat.value;
                    if (debugMode) Debug.Log($"Max Health increased from {player.maxHealth - stat.value} to {player.maxHealth}");
                    break;
                
                case LevelUpStatType.XpPullRange:
                    player.xpParticleSystem.endRange += PlayerMovement.baseXpPullRange * stat.value / 100f;
                    if (debugMode) Debug.Log($"XP Pull Range increased from {player.xpParticleSystem.endRange - PlayerMovement.baseXpPullRange * stat.value / 100f} to {player.xpParticleSystem.endRange}");
                    break;
                
                case LevelUpStatType.DashCooldown:
                    player.dashCooldown = Mathf.Max(0.1f, player.dashCooldown - stat.value / 100f); // Ensure cooldown doesn't go below 0.1 seconds
                    if (debugMode) Debug.Log($"Dash Cooldown decreased from {player.dashCooldown + stat.value / 100f} to {player.dashCooldown}");
                    break;

                case LevelUpStatType.DashStrength:
                    player.dashStrength += PlayerMovement.baseDashStrength * stat.value / 100f;
                    if (debugMode) Debug.Log($"Dash Strength increased from {player.dashStrength - stat.value} to {player.dashStrength}");
                    break;
                    
                case LevelUpStatType.CastSpeed:
                    player.castSpeed += stat.value / 100f;
                    if (debugMode) Debug.Log($"Cast Speed increased from {player.castSpeed - stat.value} to {player.castSpeed}");
                    break;
                    
                case LevelUpStatType.CastStrength:
                    player.castStrength += stat.value / 100f;
                    if (debugMode) Debug.Log($"Cast Strength increased from {player.castStrength- stat.value} to {player.castStrength}");
                    break;
                    
                case LevelUpStatType.SpellLevel:
                    if (stat.LevelUpSpellLevelBonus != null)
                    {
                        // Upgrade the specific spell
                        for (int i = 0; i < stat.LevelUpSpellLevelBonus.levelBonus; i++)
                        {
                            Spell.UpgradeSpell(stat.LevelUpSpellLevelBonus.spell);
                        }
                        if (debugMode) Debug.Log($"Spell Bonus of {stat.LevelUpSpellLevelBonus.spell} increased by {stat.LevelUpSpellLevelBonus}");
                    }
                    break;
            }
        }
        FindAnyObjectByType<UIManager>().updateStatTrackerUI();
        player.UpdateUI();
    }   
}
