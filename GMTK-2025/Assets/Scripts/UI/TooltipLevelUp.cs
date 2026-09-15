
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

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

    public override void OnSelect(BaseEventData baseEventData)
    {
        if(levelUpUI.panels[panelIndex].button.interactable)
        {
            CreateStatChangeMessage();
            TooltipManager._instance.SetAndShowTooltip(message);
            TooltipManager._instance.UpdateTooltipPosition(transform.position);        
        }
    }

    public override void OnDeselect(BaseEventData baseEventData)
    {
        TooltipManager._instance.HideTooltip();
    }


    private void CreateStatChangeMessage()
    {
        message = string.Empty;
        string messageColor = DetermineMessageColor(statContainers.panelStats[panelIndex].statRarity);
        foreach (var stat in statContainers.panelStats[panelIndex].stats)
        {
            switch(stat.type)
            {
                case LevelUpStatType.Speed:
                    message += $"Speed: {player.maxSpeed} -> <color={messageColor}>{player.maxSpeed + stat.value}</color>\n";
                    break;
                case LevelUpStatType.Health:
                    message += $"Health: {player.maxHealth} -> <color={messageColor}>{player.maxHealth + stat.value}</color>\n";
                    break;
                case LevelUpStatType.DashStrength:
                    message += $"Dash Strength: {Math.Round(player.dashStrength, 2)} -> <color={messageColor}>{Math.Round(player.dashStrength + PlayerMovement.baseDashStrength * stat.value / 100f, 2)}</color>\n";
                    break;
                case LevelUpStatType.CastSpeed:
                    message += $"Cast Speed: {Math.Round(player.castSpeed, 2)} -> <color={messageColor}>{Math.Round(player.castSpeed + stat.value / 100f, 2)}</color>\n";
                    break;
                case LevelUpStatType.CastStrength:
                    message += $"Cast Strength: {Math.Round(player.castStrength, 2)} -> <color={messageColor}>{Math.Round(player.castStrength + stat.value / 100f, 2)}</color>\n";
                    break;
                case LevelUpStatType.DashCooldown:
                    message += $"Dash Cooldown: {Math.Round(player.dashCooldown, 2)} -> <color={messageColor}>{Math.Round(Mathf.Max(0.1f, player.dashCooldown - stat.value / 100f), 2)}</color>\n";
                    break;
                case LevelUpStatType.XpPullRange:
                    message += $"XP Pull Range: {Math.Round(player.xpParticleSystem.endRange, 2)} -> <color={messageColor}>{Math.Round(player.xpParticleSystem.endRange + PlayerMovement.baseXpPullRange * stat.value / 100f, 2)}</color>\n";
                    break;
                case LevelUpStatType.SpellLevel:
                    if (stat.LevelUpSpellLevelBonus != null)
                    {
                        message += $"{stat.LevelUpSpellLevelBonus.spell}: Lv.{Spell.GetSpellLevel(stat.LevelUpSpellLevelBonus.spell)} -> <color={messageColor}>Lv.{Spell.GetSpellLevel(stat.LevelUpSpellLevelBonus.spell) + stat.value}</color>\n";   
                    }
                    break;
                default:
                    message += $"No Upgrade Here";
                    break;
            }
        }
    }

    private string DetermineMessageColor(StatRarity statRarity)
    {
        switch(statRarity)
        {
            case StatRarity.Common:
                return "#808080ff";
            case StatRarity.Uncommon:
                return "#00df4aff";
            case StatRarity.Rare:
                return "#0011ffff";
            case StatRarity.Epic:
                return "#ec00ecff";
            case StatRarity.Legendary:
                return "#ff1e00ff";
            default:
                return "white";
        }
    }
}
