using System;
using UnityEngine;

public class SpellCombinations : MonoBehaviour
{
    [SerializeField] private Spell[] combinationSpellPrefabs; // Prefab for the combination spell
    public Spell OutputSpellCombination(Spell spellOne, Spell spellTwo)
    {

        // For now, we will deal with this. It's probably better to use a dictionary or something similar for combinations.
        switch (spellOne.spellType1, spellTwo.spellType1)
        {
            // Fire combinations
            case (Spell.SpellType.Fire, Spell.SpellType.Fire):
                Debug.Log("Combination: Explosive Shot");
                return combinationSpellPrefabs[0];
            case (Spell.SpellType.Fire, Spell.SpellType.Water):
                Debug.Log("Combination: Steam Vent");
                return combinationSpellPrefabs[1];
            case (Spell.SpellType.Fire, Spell.SpellType.Lightning):
                Debug.Log("Combination: Fissure Flare");
                return combinationSpellPrefabs[2];
            case (Spell.SpellType.Fire, Spell.SpellType.Dark):
                Debug.Log("Combination: Ghostflame");
                return combinationSpellPrefabs[3];

            // Water combinations
            case (Spell.SpellType.Water, Spell.SpellType.Fire):
                Debug.Log("Combination: Steam Vent");
                return combinationSpellPrefabs[4];
            case (Spell.SpellType.Water, Spell.SpellType.Water):
                Debug.Log("Combination: Wave");
                return combinationSpellPrefabs[5];
            case (Spell.SpellType.Water, Spell.SpellType.Lightning):
                Debug.Log("Combination: Chain Lightning");
                return combinationSpellPrefabs[6];
            case (Spell.SpellType.Water, Spell.SpellType.Dark):
                Debug.Log("Combination: Poison Puddles");
                return combinationSpellPrefabs[7];

            // Lightning combinations
            case (Spell.SpellType.Lightning, Spell.SpellType.Fire):
                Debug.Log("Combination: Fissure Flare");
                return combinationSpellPrefabs[8];
            case (Spell.SpellType.Lightning, Spell.SpellType.Water):
                Debug.Log("Combination: Chain Lightning");
                return combinationSpellPrefabs[9];
            case (Spell.SpellType.Lightning, Spell.SpellType.Lightning):
                Debug.Log("Combination: Storm");
                return combinationSpellPrefabs[10];
            case (Spell.SpellType.Lightning, Spell.SpellType.Dark):
                Debug.Log("Combination: Black Flash");
                return combinationSpellPrefabs[11];

            // Dark combinations
            case (Spell.SpellType.Dark, Spell.SpellType.Fire):
                Debug.Log("Combination: Ghostflame");
                return combinationSpellPrefabs[12];
            case (Spell.SpellType.Dark, Spell.SpellType.Water):
                Debug.Log("Combination: Poison Puddles");
                return combinationSpellPrefabs[13];
            case (Spell.SpellType.Dark, Spell.SpellType.Lightning):
                Debug.Log("Combination: Black Flash");
                return combinationSpellPrefabs[14];
            case (Spell.SpellType.Dark, Spell.SpellType.Dark):
                Debug.Log("Combination: Black Hole");
                return combinationSpellPrefabs[15];
            default:
                Debug.Log("No valid combination found.");
                return null;
        }
    }
}
