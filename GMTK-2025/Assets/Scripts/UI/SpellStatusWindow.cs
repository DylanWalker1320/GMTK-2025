using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellStatusWindow : MonoBehaviour
{
    [Header("Spell Cells")]
    [SerializeField] private SpellStatusCell[] cellObjects;
    [Header("Spell Information Container")]
    public Image[] spellFormula = new Image[2];
    [SerializeField] private GameObject formulaContainer;
    [SerializeField] private TextMeshProUGUI spellName;
    [SerializeField] private TextMeshProUGUI spellLevel;
    [SerializeField] private TextMeshProUGUI damage;
    [SerializeField] private TextMeshProUGUI description;

    void RefreshWindow() // Called via buttons
    {
        UpdateLevels();
    }

    void UpdateLevels()
    {
        foreach(SpellStatusCell cell in cellObjects)
        {
            cell.RefreshCell();
        }
    }

    public void DisplayText(SpellStatusCell targetCell) // Called via triggers
    {
        Spell spellData = targetCell.spellPrefabData;
        spellName.text = spellData.spell.ToString();
        spellLevel.text = Spell.GetSpellLevel(spellData.spell).ToString();
        damage.text = spellData.CalculateDamage(spellData.GetDamage(), spellData.spellType1, spellData.spellType2, false).ToString();
        description.text = targetCell.spellDescription;

        if(targetCell.isComboSpell)
        {
            formulaContainer.SetActive(true);
            spellFormula[0].sprite = targetCell.spellSpecificFormula[0];
            spellFormula[1].sprite = targetCell.spellSpecificFormula[1];
        }
        else
        {
            formulaContainer.SetActive(false);
        }
    }
}
