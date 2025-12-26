
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipLevelUp : Tooltip
{
    private StatGenerator statContainers;
    private PlayerMovement player;
    private LevelUpUI levelUpUI;
    [SerializeField] private int panelIndex;

    private void Awake()
    {
        statContainers = FindAnyObjectByType<StatGenerator>();
        player = FindAnyObjectByType<PlayerMovement>();
        levelUpUI = FindAnyObjectByType<LevelUpUI>();

    }
    public override void OnMouseDown()
    {
        TooltipManager._instance.SetAndShowTooltip(message);
    }

    public override void OnMouseExit()
    {
        TooltipManager._instance.HideTooltip();
    }

    public override void OnPointerEnter(PointerEventData pointerEventData)
    {
        if(levelUpUI.panels[panelIndex].button.interactable)
        {
            CreateStatChangeMessage();
            TooltipManager._instance.SetAndShowTooltip(message);            
        }

    }

    public override void OnPointerExit(PointerEventData pointerEventData)
    {
        TooltipManager._instance.HideTooltip();
    }


    private void CreateStatChangeMessage()
    {
        message = string.Empty;
        foreach (var stat in statContainers.panelStats[panelIndex].stats)
        {
            switch(stat.type)
            {
                case LevelUpStatType.Speed:
                    message += $"Speed: {player.maxSpeed} -> {player.maxSpeed + stat.value}\n";
                    break;
                case LevelUpStatType.Health:
                    message += $"Health: {player.maxHealth} -> {player.maxHealth + stat.value}\n";
                    break;
                case LevelUpStatType.CastSpeed:
                    message += $"Cast Speed: {Mathf.Round(player.castSpeed * 100.00f) * 0.01f} -> {Mathf.Round((player.castSpeed + stat.value) * 100.00f) * 0.01f}\n";
                    break;
                case LevelUpStatType.CastStrength:
                    message += $"Cast Strength: {Mathf.Round(player.castStrength * 100.00f) * 0.01f} -> {Mathf.Round((player.castStrength + stat.value) * 100.00f) * 0.01f}\n";
                    break;
                case LevelUpStatType.SpellLevel:
                    if (stat.LevelUpSpellLevelBonus != null)
                    {
                        message += $"{stat.LevelUpSpellLevelBonus.spell}: Lv.{Spell.GetSpellLevel(stat.LevelUpSpellLevelBonus.spell)} -> Lv.{Spell.GetSpellLevel(stat.LevelUpSpellLevelBonus.spell) + stat.value}\n";   
                    }
                    break;
                default:
                    message += $"No Upgrade Here";
                    break;
            }
        }
    }
}
