using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// Enums for StatRarity and LevelUpStatType
public enum StatRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public enum LevelUpStatType
{
    Speed,
    Health,
    CastSpeed,
    CastStrength,
    SpellLevel,
    DashCooldown,
    DashStrength,
    XpPullRange
}

public class LevelUpSpellLevelBonus
{
    public Spell.Spells spell;
    public int levelBonus;

    public LevelUpSpellLevelBonus(Spell.Spells spell, int levelBonus)
    {
        this.spell = spell;
        this.levelBonus = levelBonus;
    }

    public override string ToString()
    {
        return $"+{levelBonus} to {spell}";
    }
}

[Serializable]
public class LevelUpStatDist // Needs Renaming. Class for individual stat containing their stat type and corresponding value for an externally defined rarity type
{
    public LevelUpStatType type;
    public float value;
    public LevelUpSpellLevelBonus LevelUpSpellLevelBonus;

    public LevelUpStatDist(LevelUpStatType type, float value)
    {
        this.type = type;
        this.value = value;
        this.LevelUpSpellLevelBonus = null;
    }

    public LevelUpStatDist(LevelUpStatType type, LevelUpSpellLevelBonus spellBonus)
    {
        this.type = type;
        this.value = spellBonus.levelBonus;
        this.LevelUpSpellLevelBonus = spellBonus;
    }

    public override string ToString()
    {
        return type switch
        {
            LevelUpStatType.CastSpeed => $"+{value}% Cast Speed",
            LevelUpStatType.CastStrength => $"+{value}% Cast Strength",
            LevelUpStatType.DashStrength => $"+{value}% Dash Strength",
            LevelUpStatType.DashCooldown => $"-{value}% Dash Cooldown",
            LevelUpStatType.XpPullRange => $"+{value}% XP Pull Range",
            LevelUpStatType.SpellLevel => LevelUpSpellLevelBonus != null ? LevelUpSpellLevelBonus.ToString() : $"+{value} Spell Level",
            LevelUpStatType.Speed => $"+ {value} Speed",
            LevelUpStatType.Health => $"+{value} Health",
            _ => $"{type}: {value}"
        };        
    }
}

[SerializeField]
public class GeneratedStat // class for the generated stat list and the overall rarity
{
    public string statName;
    public StatRarity statRarity;
    public List<LevelUpStatDist> stats;

    public GeneratedStat(string name, StatRarity statRarity, List<LevelUpStatDist> stats)
    {
        this.statName = statRarity.ToString();
        this.statRarity = statRarity;
        this.stats = stats;
    }
    public override string ToString()
    {
        string output = $"{statName} (Rarity: {statRarity})\nStats:\n";
        foreach (var stat in stats)
        {
            output += $"- {stat}\n";
        }
        return output;
    }
}

public static class LevelUpStatDefinitions
{
    // StatRarity chances (out of 100)
    public static readonly Dictionary<StatRarity, int> StatRarityWeights = new()
    {
        { StatRarity.Common,    40 },
        { StatRarity.Uncommon,  30 },
        { StatRarity.Rare,      15 },
        { StatRarity.Epic,      10 },
        { StatRarity.Legendary, 5  }
    };

    // Number of stat lines per StatRarity
    public static readonly Dictionary<StatRarity, int> StatCountByStatRarity = new()
    {
        { StatRarity.Common,    1 },
        { StatRarity.Uncommon,  1 },
        { StatRarity.Rare,      2 },
        { StatRarity.Epic,      2 },
        { StatRarity.Legendary, 3 }
    };

    public static readonly Dictionary<StatRarity, int> BaseStatSoulCost = new()
    {
        { StatRarity.Common,    20 },
        { StatRarity.Uncommon,  30 },
        { StatRarity.Rare,      50 },
        { StatRarity.Epic,      75 },
        { StatRarity.Legendary, 100}
    };

    // Stat weights by StatRarity (Weights are in n% form because WeightedList uses int weights)
    public static readonly Dictionary<StatRarity, Dictionary<LevelUpStatType, int>> StatWeightsByStatRarity = new()
    {
        { 
            StatRarity.Common, new Dictionary<LevelUpStatType, int>
            {
                { LevelUpStatType.Speed,         24 },
                { LevelUpStatType.Health,        20 },
                { LevelUpStatType.XpPullRange,   14 },
                { LevelUpStatType.DashStrength,  10 },
                { LevelUpStatType.DashCooldown,  8  },
                { LevelUpStatType.CastSpeed,     8  },
                { LevelUpStatType.CastStrength,  8  },
                { LevelUpStatType.SpellLevel,    8  }
            }
        },
        { 
            StatRarity.Uncommon, new Dictionary<LevelUpStatType, int>
            {
                { LevelUpStatType.Speed,         13 },
                { LevelUpStatType.Health,        18 },
                { LevelUpStatType.XpPullRange,   20 },
                { LevelUpStatType.DashStrength,  16 },
                { LevelUpStatType.DashCooldown,  11 },
                { LevelUpStatType.CastSpeed,     8  },
                { LevelUpStatType.CastStrength,  7  },
                { LevelUpStatType.SpellLevel,    7  }
            }
        },
        { 
            StatRarity.Rare, new Dictionary<LevelUpStatType, int>
            {
                { LevelUpStatType.Speed,         7  },
                { LevelUpStatType.Health,        10 },
                { LevelUpStatType.XpPullRange,   14 },
                { LevelUpStatType.DashStrength,  19 },
                { LevelUpStatType.DashCooldown,  19 },
                { LevelUpStatType.CastSpeed,     14 },
                { LevelUpStatType.CastStrength,  10 },
                { LevelUpStatType.SpellLevel,    7  }
            }
        },
        { 
            StatRarity.Epic, new Dictionary<LevelUpStatType, int>
            {
                { LevelUpStatType.Speed,         7  },
                { LevelUpStatType.Health,        7  },
                { LevelUpStatType.XpPullRange,   8  },
                { LevelUpStatType.DashStrength,  11 },
                { LevelUpStatType.DashCooldown,  16 },
                { LevelUpStatType.CastSpeed,     20 },
                { LevelUpStatType.CastStrength,  18 },
                { LevelUpStatType.SpellLevel,    13 }
            }
        },
        { 
            StatRarity.Legendary, new Dictionary<LevelUpStatType, int>
            {
                { LevelUpStatType.Speed,         8  },
                { LevelUpStatType.Health,        8  },
                { LevelUpStatType.XpPullRange,   8  },
                { LevelUpStatType.DashStrength,  8  },
                { LevelUpStatType.DashCooldown,  10 },
                { LevelUpStatType.CastSpeed,     14 },
                { LevelUpStatType.CastStrength,  20 },
                { LevelUpStatType.SpellLevel,    24 }
            }
        }
    };

    // Stat ranges by StatRarity and stat type
    public static float GetStatValue(LevelUpStatType statType, StatRarity rarity)
    {
        return statType switch
        {
            LevelUpStatType.Speed => rarity switch
            {
                StatRarity.Common =>    2,
                StatRarity.Uncommon =>  4,
                StatRarity.Rare =>      6,
                StatRarity.Epic =>      8,
                StatRarity.Legendary => 10,
                _ => 2
            },
            LevelUpStatType.Health => rarity switch
            {
                StatRarity.Common =>    Random.Range(5, 15),
                StatRarity.Uncommon =>  Random.Range(5, 25),
                StatRarity.Rare =>      Random.Range(10, 30),
                StatRarity.Epic =>      Random.Range(20, 40),
                StatRarity.Legendary => Random.Range(30, 50),
                _ => 5
            },
            LevelUpStatType.XpPullRange => rarity switch
            {
                StatRarity.Common =>    Random.Range(1, 5),
                StatRarity.Uncommon =>  Random.Range(6, 10),
                StatRarity.Rare =>      Random.Range(11, 15),
                StatRarity.Epic =>      Random.Range(16, 20),
                StatRarity.Legendary => Random.Range(21, 25),
                _ => 5
            },
            LevelUpStatType.DashStrength => rarity switch
            {
                StatRarity.Common =>    Random.Range(1, 5),
                StatRarity.Uncommon =>  Random.Range(6, 10),
                StatRarity.Rare =>      Random.Range(11, 15),
                StatRarity.Epic =>      Random.Range(16, 20),
                StatRarity.Legendary => Random.Range(21, 25),
                _ => 5
            },
            LevelUpStatType.DashCooldown => rarity switch
            {
                StatRarity.Common =>    Random.Range(1, 2),
                StatRarity.Uncommon =>  Random.Range(3, 4),
                StatRarity.Rare =>      Random.Range(5, 6),
                StatRarity.Epic =>      Random.Range(7, 8),
                StatRarity.Legendary => Random.Range(9, 10),
                _ => 5
            },
            LevelUpStatType.CastSpeed => rarity switch
            {
                StatRarity.Common =>    Random.Range(1, 5),
                StatRarity.Uncommon =>  Random.Range(6, 10),
                StatRarity.Rare =>      Random.Range(11, 15),
                StatRarity.Epic =>      Random.Range(16, 20),
                StatRarity.Legendary => Random.Range(21, 25),
                _ => 5
            },
            LevelUpStatType.CastStrength => rarity switch
            {
                StatRarity.Common =>    Random.Range(1, 4),
                StatRarity.Uncommon =>  Random.Range(5, 8),
                StatRarity.Rare =>      Random.Range(9, 12),
                StatRarity.Epic =>      Random.Range(13, 16),
                StatRarity.Legendary => Random.Range(17, 20),
                _ => 5
            },
            LevelUpStatType.SpellLevel => rarity switch
            {
                StatRarity.Common =>    1,
                StatRarity.Uncommon =>  Random.Range(1f, 2f) >= 1.25f ? 2 : 1, // 25% chance for +2
                StatRarity.Rare =>      Random.Range(1f, 2f) >= 1.5f  ? 2 : 1, // 50% chance for +2
                StatRarity.Epic =>      Random.Range(1f, 2f) >= 1.75f ? 2 : 1, // 75% chance for +2
                StatRarity.Legendary => Random.Range(1f, 2f) >= 1.5f  ? 2 : 3, // 50% chance for +3
                _ => 0
            },
            _ => 0
        };
    }

    // Get a random spell for spell level bonuses
    public static Spell.Spells GetRandomSpell()
    {
        var allSpells = System.Enum.GetValues(typeof(Spell.Spells));
        return (Spell.Spells)allSpells.GetValue(Random.Range(0, allSpells.Length));
    }
}
