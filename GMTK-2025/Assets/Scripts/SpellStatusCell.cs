
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellStatusCell : MonoBehaviour
{
    public Image spellSprite;
    public Spell spellPrefabData;
    public TextMeshProUGUI levelText;
    public string spellDescription;
    public Sprite[] spellSpecificFormula = new Sprite[2];
    public bool isComboSpell = true;
    
    public void RefreshCell()
    {
        levelText.text = "Lvl "+ Spell.GetSpellLevel(spellPrefabData.spell);
    }
}
