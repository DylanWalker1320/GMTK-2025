using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class TooltipThreeUpgrades : Tooltip
{
    [SerializeField] private ThreeUpgradeScreen threeUpgradeScreenReference;
    [SerializeField] private bool isHealType;
    [SerializeField] private bool isStatType;
    [SerializeField] private bool isSpellType;
    [SerializeField] private Vector2 tooltipOffset;
    private static PlayerMovement playerMovement;

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
        DesignateMessageType();
        TooltipManager._instance.SetAndShowTooltip(message);
    }

    public override void OnPointerExit(PointerEventData pointerEventData)
    {
        TooltipManager._instance.HideTooltip();
    }

    public override void OnSelect(BaseEventData baseEventData)
    {
        DesignateMessageType();
        TooltipManager._instance.UpdateTooltipPosition(transform.position + (Vector3)tooltipOffset);
        TooltipManager._instance.SetAndShowTooltip(message);
    }

    public override void OnDeselect(BaseEventData baseEventData)
    {
        TooltipManager._instance.HideTooltip();
    }

    private void DesignateMessageType()
    {
        if (playerMovement == null)
        {
            playerMovement = FindFirstObjectByType<PlayerMovement>();
        }
        
        if (isHealType)
        {
            float newHealth = Mathf.Clamp(threeUpgradeScreenReference.healAmount + playerMovement.health, 0, playerMovement.maxHealth);
            message = $"<u><color=green>Restore Health</color></u>\n\n{playerMovement.health} -> <color=green>{newHealth}</color>\n";
        }
        else if (isStatType)
        {
            switch(threeUpgradeScreenReference.upgradeStatType)
            {
                case ThreeUpgradeScreen.StatIncreaseType.Health:
                    message = $"<u><color=yellow>Max Health Increase</color></u>\n\n{playerMovement.maxHealth} -> <color=yellow>{playerMovement.maxHealth + threeUpgradeScreenReference.healthUpgradeIncrease}</color>\n";
                    break;
                case ThreeUpgradeScreen.StatIncreaseType.Speed:
                    message = $"<u><color=yellow>Speed Increase</color></u>\n\n{playerMovement.maxSpeed} -> <color=yellow>{playerMovement.maxSpeed + threeUpgradeScreenReference.speedUpgradeIncrease}</color>\n";
                    break;
                case ThreeUpgradeScreen.StatIncreaseType.IFrames:
                    message = $"<u><color=yellow>Invincibility Frames Increase</color></u>\n\n{playerMovement.invincibilityFrames} -> <color=yellow>{playerMovement.invincibilityFrames + threeUpgradeScreenReference.iFramesUpgradeIncrease}</color>\n";
                    break;
                case ThreeUpgradeScreen.StatIncreaseType.CastSpeed:
                    message = $"<u><color=yellow>Cast Speed Increase</color></u>\n\n{Math.Round(playerMovement.castSpeed, 2)} -> <color=yellow>{Math.Round(playerMovement.castSpeed + threeUpgradeScreenReference.castSpeedUpgradeIncrease, 2)}</color>\n";
                    break;
                case ThreeUpgradeScreen.StatIncreaseType.CastStrength:
                    message = $"<u><color=yellow>Cast Strength Increase</color></u>\n\n{Math.Round(playerMovement.castStrength, 2)} -> <color=yellow>{Math.Round(playerMovement.castStrength + threeUpgradeScreenReference.castStrengthUpgradeIncrease, 2)}</color>\n";
                    break;
                case ThreeUpgradeScreen.StatIncreaseType.DashCooldown:
                    message = $"<u><color=yellow>Dash Cooldown Decrease</color></u>\n\n{Math.Round(playerMovement.dashCooldown, 2)} -> <color=yellow>{Math.Round(Mathf.Max(0.1f, playerMovement.dashCooldown - threeUpgradeScreenReference.dashCooldownUpgradeIncrease / 100f), 2)}</color>\n";
                    break;
                case ThreeUpgradeScreen.StatIncreaseType.DashStrength:
                    message = $"<u><color=yellow>Dash Strength Increase</color></u>\n\n{Math.Round(playerMovement.dashStrength, 2)} -> <color=yellow>{Math.Round(playerMovement.dashStrength + PlayerMovement.baseDashStrength * threeUpgradeScreenReference.dashStrengthUpgradeIncrease / 100f, 2)}</color>\n";
                    break;
                case ThreeUpgradeScreen.StatIncreaseType.XpPullRange:
                    message = $"<u><color=yellow>XP Pull Range Increase</color></u>\n\n{Math.Round(playerMovement.xpParticleSystem.endRange, 2)} -> <color=yellow>{Math.Round(playerMovement.xpParticleSystem.endRange + PlayerMovement.baseXpPullRange * threeUpgradeScreenReference.xpPullRangeUpgradeIncrease / 100f, 2)}</color>\n";
                    break;
                default:
                    Debug.LogError("Invalid upgrade index for stats.");
                    break;
            }
        }
        else if (isSpellType)
        {
            switch(threeUpgradeScreenReference.upgradeSpriteType)
            {
                case ThreeUpgradeScreen.SpriteType.Fire:
                    message = "<u><color=red>Fireball Spell</color></u>\n\n<color=red>Fire</color> Projectile [Single Target]";
                    break;
                case ThreeUpgradeScreen.SpriteType.Water:
                    message = "<u><color=blue>Waterball Spell</color></u>\n\n<color=blue>Water</color> Projectile [Single Target]";
                    break;
                case ThreeUpgradeScreen.SpriteType.Lightning:
                    message = "<u><color=yellow>Lightning Spell</color></u>\n\nFast <color=yellow>Lightning</color> Projectile [Nearest Target]";
                    break;
                case ThreeUpgradeScreen.SpriteType.Dark:
                    message = "<u><color=purple>Dark Energy Spell</color></u>\n\nSlow <color=purple>Dark</color> Projectile [Single Target]";
                    break;
                default:
                    Debug.LogError("Invalid upgrade index for spells.");
                    break;
            }
        }
    }
}
