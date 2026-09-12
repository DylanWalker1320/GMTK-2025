using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/* 
Hat Rarity Chances:
Common: 30%
Uncommon: 25%
Rare: 20%
Epic: 15%
Legendary: 10%

Number Of Stat Lines Per Rarity:
Common: 1
Uncommon: 2
Rare: 2
Epic: 3
Legendary: 4

Hat Stat Weighting by Rarity (%):
                 | Common | Uncommon | Rare | Epic | Legendary |
Speed:           |  0.36  |   0.17   | 0.05 | 0.03 |   0.03    |
Health:          |  0.29  |   0.25   | 0.12 | 0.04 |   0.03    |
Dash Strength:   |  0.17  |   0.25   | 0.21 | 0.09 |   0.04    |
Dash Cooldown:   |  0.08  |   0.17   | 0.24 | 0.17 |   0.08    |
Cast Speed:      |  0.04  |   0.09   | 0.21 | 0.25 |   0.17    |
Cast Strength:   |  0.03  |   0.04   | 0.12 | 0.25 |   0.29    |
Spell Level:     |  0.03  |   0.03   | 0.05 | 0.17 |   0.36    |

Hat Stat Ranges by Rarity:
               | Common | Uncommon |  Rare  |  Epic  | Legendary |
Speed:         |   2    |    4     |   6    |   8    |     10    | // Unfinshed, check the speed values post-nerf later
Health:        |  5-15  |   5-25   | 10-30  | 20-40  |   30-50   |
Cast Speed:    |  0.05  |   0.1    |  0.15  |  0.2   |    0.25   | // This is in %, so Legendary gives 25% cast speed increase
Cast Strength: |   0    |   0.05   |  0.1   |  0.15  |    0.2    | // This is in %, so Legendary gives 20% cast strength increase
Spell Level:   |  N/A   |    1     |  1-2   |  1-2   |    1-3    |

Accurate as of Dec 18, 2025
*/

// ===== Enums =====
public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public enum StatType
{
    Speed,
    Health,
    CastSpeed,
    CastStrength,
    SpellLevel,
    DashStrength,
    DashCooldown
}

// ===== Data Classes =====
public class HatColors
{
    private static readonly Dictionary<Rarity, Color> RarityColors = new()
    {
        { Rarity.Common,    ColorUtility.TryParseHtmlString("#C0C0C0", out Color commonColor) ? commonColor : Color.gray },
        { Rarity.Uncommon,  ColorUtility.TryParseHtmlString("#00FF00", out Color uncommonColor) ? uncommonColor : Color.green },
        { Rarity.Rare,      ColorUtility.TryParseHtmlString("#0000FF", out Color rareColor) ? rareColor : Color.blue },
        { Rarity.Epic,      ColorUtility.TryParseHtmlString("#ff00ff", out Color epicColor) ? epicColor : Color.magenta },
        { Rarity.Legendary, ColorUtility.TryParseHtmlString("#FFA500", out Color legendaryColor) ? legendaryColor : new Color(1f, 0.65f, 0f) }
    };

    private static readonly Dictionary<StatType, Color> StatTypeColors = new()
    {
        { StatType.Speed,        ColorUtility.TryParseHtmlString("#fff201", out Color speedColor) ? speedColor : Color.yellow },
        { StatType.Health,       ColorUtility.TryParseHtmlString("#00FF00", out Color healthColor) ? healthColor : Color.green },
        { StatType.CastSpeed,    ColorUtility.TryParseHtmlString("#3c3cff", out Color castSpeedColor) ? castSpeedColor : Color.blue },
        { StatType.CastStrength, ColorUtility.TryParseHtmlString("#ff0000", out Color castStrengthColor) ? castStrengthColor : Color.red },
        { StatType.SpellLevel,   ColorUtility.TryParseHtmlString("#ff2ed5", out Color spellLevelColor) ? spellLevelColor : Color.magenta },
        { StatType.DashStrength, ColorUtility.TryParseHtmlString("#3ff8e2", out Color dashStrengthColor) ? dashStrengthColor : Color.cyan },
        { StatType.DashCooldown, ColorUtility.TryParseHtmlString("#ff6f00", out Color dashCooldownColor) ? dashCooldownColor : new Color(1f, 0.43f, 0f) }
    };

    public static Color GetRarityColor(Rarity rarity)
    {
        return RarityColors[rarity];
    }

    public static Color GetStatTypeColor(StatType statType)
    {
        return StatTypeColors[statType];
    }
}

[Serializable]
public class SpellLevelBonus
{
    public Spell.Spells spell;
    public int levelBonus;

    public SpellLevelBonus(Spell.Spells spell, int levelBonus)
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
public class HatStat
{
    public StatType type;
    public float value;
    public SpellLevelBonus spellBonus; // Only used when type is SpellLevel

    public HatStat(StatType type, float value)
    {
        this.type = type;
        this.value = value;
        this.spellBonus = null;
    }

    public HatStat(StatType type, SpellLevelBonus spellBonus)
    {
        this.type = type;
        this.value = spellBonus.levelBonus;
        this.spellBonus = spellBonus;
    }

    public override string ToString()
    {
        return type switch
        {
            StatType.CastSpeed => $"+{value}% Cast Speed",
            StatType.CastStrength => $"+{value}% Cast Strength",
            StatType.SpellLevel => spellBonus != null ? spellBonus.ToString() : $"+{value} Spell Level",
            StatType.Speed => $"+{value} Speed",
            StatType.Health => $"+{value} Health",
            StatType.DashStrength => $"+{value}% Dash Strength",
            StatType.DashCooldown => $"-{value}% Dash Cooldown",
            _ => $"{type}: {value}"
        };
    }
}

[Serializable]
public struct HatComponents
{
    public GameObject hatType;
    public Sprite pattern;
    public Color color;
}

[Serializable]
public class GeneratedHat
{
    public string hatName;
    public Rarity rarity;
    public List<HatStat> stats;
    public HatComponents components;
    public override string ToString()
    {
        string output = $"{hatName} (Rarity: {rarity})\nStats:\n";
        foreach (var stat in stats)
        {
            output += $"- {stat}\n";
        }
        return output;
    }

    public string PrintStatsOnly()
    {
        string output = "";
        foreach (var stat in stats)
        {
            output += $"- {stat}\n";
        }
        return output;
    }

    public GeneratedHat(string name, Rarity rarity, List<HatStat> stats)
    {
        this.hatName = name;
        this.rarity = rarity;
        this.stats = stats;
    }
}

// ===== Stat Definitions =====
public static class HatStatDefinitions
{
    // Rarity chances (out of 100)
    public static readonly Dictionary<Rarity, int> RarityWeights = new()
    {
        { Rarity.Common,    30 },
        { Rarity.Uncommon,  25 },
        { Rarity.Rare,      20 },
        { Rarity.Epic,      15 },
        { Rarity.Legendary, 10 }
    };

    // Number of stat lines per rarity
    public static readonly Dictionary<Rarity, int> StatCountByRarity = new()
    {
        { Rarity.Common,    1 },
        { Rarity.Uncommon,  2 },
        { Rarity.Rare,      2 },
        { Rarity.Epic,      3 },
        { Rarity.Legendary, 4 }
    };

    // Stat weights by rarity (Weights are in n% form because WeightedList uses int weights)
    public static readonly Dictionary<Rarity, Dictionary<StatType, int>> StatWeightsByRarity = new()
    {
        { 
            // Stats are ordered by their relative strength. Weight is distributed roughly as a normal curve centred on a middle stat for that rarity tier.
            Rarity.Common, new Dictionary<StatType, int>
            {
                { StatType.Speed,        36 },
                { StatType.Health,       29 },
                { StatType.DashStrength, 17 },
                { StatType.DashCooldown, 8  },
                { StatType.CastSpeed,    4  },
                { StatType.CastStrength, 3  },
                { StatType.SpellLevel,   3  }
            }
        },
        { 
            Rarity.Uncommon, new Dictionary<StatType, int>
            {
                { StatType.Speed,        17 },
                { StatType.Health,       25 },
                { StatType.DashStrength, 25 },
                { StatType.DashCooldown, 17 },
                { StatType.CastSpeed,    9  },
                { StatType.CastStrength, 4  },
                { StatType.SpellLevel,   3  }
            }
        },
        { 
            Rarity.Rare, new Dictionary<StatType, int>
            {
                { StatType.Speed,        5  },
                { StatType.Health,       12 },
                { StatType.DashStrength, 21 },
                { StatType.DashCooldown, 24 },
                { StatType.CastSpeed,    21 },
                { StatType.CastStrength, 12 },
                { StatType.SpellLevel,   5  }
            }
        },
        { 
            Rarity.Epic, new Dictionary<StatType, int>
            {
                { StatType.Speed,        3  },
                { StatType.Health,       4  },
                { StatType.DashStrength, 9  },
                { StatType.DashCooldown, 17 },
                { StatType.CastSpeed,    25 },
                { StatType.CastStrength, 25 },
                { StatType.SpellLevel,   17 }
            }
        },
        { 
            Rarity.Legendary, new Dictionary<StatType, int>
            {
                { StatType.Speed,        3  },
                { StatType.Health,       3  },
                { StatType.DashStrength, 4  },
                { StatType.DashCooldown, 8  },
                { StatType.CastSpeed,    17 },
                { StatType.CastStrength, 29 },
                { StatType.SpellLevel,   36 }
            }
        }
    };

    // Stat ranges by rarity and stat type
    public static int GetStatValue(StatType statType, Rarity rarity)
    {
        return statType switch
        {
            StatType.Speed => rarity switch
            {
                Rarity.Common =>    2,
                Rarity.Uncommon =>  4,
                Rarity.Rare =>      6,
                Rarity.Epic =>      8,
                Rarity.Legendary => 10,
                _ => 2
            },
            StatType.Health => rarity switch
            {
                Rarity.Common =>    Random.Range(5, 15),
                Rarity.Uncommon =>  Random.Range(5, 25),
                Rarity.Rare =>      Random.Range(10, 30),
                Rarity.Epic =>      Random.Range(20, 40),
                Rarity.Legendary => Random.Range(30, 50),
                _ => 5
            },
            StatType.DashStrength => rarity switch
            {
                Rarity.Common =>    Random.Range(1, 5),
                Rarity.Uncommon =>  Random.Range(6, 10),
                Rarity.Rare =>      Random.Range(11, 15),
                Rarity.Epic =>      Random.Range(16, 20),
                Rarity.Legendary => Random.Range(21, 25),
                _ => 5
            },
            StatType.DashCooldown => rarity switch
            {
                Rarity.Common =>    Random.Range(1, 2),
                Rarity.Uncommon =>  Random.Range(3, 4),
                Rarity.Rare =>      Random.Range(5, 6),
                Rarity.Epic =>      Random.Range(7, 8),
                Rarity.Legendary => Random.Range(9, 10),
                _ => 5
            },
            StatType.CastSpeed => rarity switch
            {
                Rarity.Common =>    Random.Range(1, 5),
                Rarity.Uncommon =>  Random.Range(6, 10),
                Rarity.Rare =>      Random.Range(11, 15),
                Rarity.Epic =>      Random.Range(16, 20),
                Rarity.Legendary => Random.Range(21, 25),
                _ => 5
            },
            StatType.CastStrength => rarity switch
            {
                Rarity.Common =>    Random.Range(1, 4),
                Rarity.Uncommon =>  Random.Range(5, 8),
                Rarity.Rare =>      Random.Range(9, 12),
                Rarity.Epic =>      Random.Range(13, 16),
                Rarity.Legendary => Random.Range(17, 20),
                _ => 5
            },
            StatType.SpellLevel => rarity switch
            {
                Rarity.Common =>    1,
                Rarity.Uncommon =>  Random.Range(1f, 2f) >= 1.25f ? 2 : 1, // 25% chance for +2
                Rarity.Rare =>      Random.Range(1f, 2f) >= 1.5f  ? 2 : 1, // 50% chance for +2
                Rarity.Epic =>      Random.Range(1f, 2f) >= 1.75f ? 2 : 1, // 75% chance for +2
                Rarity.Legendary => Random.Range(1f, 2f) >= 1.5f  ? 2 : 3, // 50% chance for +3
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