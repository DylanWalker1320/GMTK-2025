
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
                    message += $"Dash Strength: {Mathf.Round(player.dashStrength * 100.00f) * 0.01f} -> <color={messageColor}>{Mathf.Round((player.dashStrength + stat.value) * 100.00f) * 0.01f}</color>\n";
                    break;
                case LevelUpStatType.CastSpeed:
                    message += $"Cast Speed: {Mathf.Round(player.castSpeed * 100.00f) * 0.01f} -> <color={messageColor}>{Mathf.Round((player.castSpeed + stat.value) * 100.00f) * 0.01f}</color>\n";
                    break;
                case LevelUpStatType.CastStrength:
                    message += $"Cast Strength: {Mathf.Round(player.castStrength * 100.00f) * 0.01f} -> <color={messageColor}>{Mathf.Round((player.castStrength + stat.value) * 100.00f) * 0.01f}</color>\n";
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
