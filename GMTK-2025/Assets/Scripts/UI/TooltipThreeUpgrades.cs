using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipThreeUpgrades : Tooltip
{
    [SerializeField] private ThreeUpgradeScreen threeUpgradeScreenReference;
    [SerializeField] private bool isHealType;
    [SerializeField] private bool isStatType;
    [SerializeField] private bool isSpellType;
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

    private void DesignateMessageType()
    {
        if (isHealType)
        {
            float newHealth = Mathf.Clamp(threeUpgradeScreenReference.healAmount + FindAnyObjectByType<PlayerMovement>().health, 0, FindAnyObjectByType<PlayerMovement>().maxHealth);
            message = $"<u><color=green>Restore Health</color></u>\n\n{FindAnyObjectByType<PlayerMovement>().health} -> <color=green>{newHealth}</color>\n";
        }
        else if (isStatType)
        {
            switch(threeUpgradeScreenReference.upgradeStatType)
            {
                case ThreeUpgradeScreen.StatIncreaseType.Health:
                    message = $"<u><color=yellow>Max Health Increase</color></u>\n\n{FindAnyObjectByType<PlayerMovement>().maxHealth} -> <color=yellow>{FindAnyObjectByType<PlayerMovement>().maxHealth + threeUpgradeScreenReference.healthUpgradeIncrease}</color>\n";
                    break;
                case ThreeUpgradeScreen.StatIncreaseType.Speed:
                    message = $"<u><color=yellow>Speed Increase</color></u>\n\n{FindAnyObjectByType<PlayerMovement>().maxSpeed} -> <color=yellow>{FindAnyObjectByType<PlayerMovement>().maxSpeed + threeUpgradeScreenReference.speedUpgradeIncrease}</color>\n";
                    break;
                case ThreeUpgradeScreen.StatIncreaseType.IFrames:
                    message = $"<u><color=yellow>Invincibility Frames Increase</color></u>\n\n{FindAnyObjectByType<PlayerMovement>().invincibilityFrames} -> <color=yellow>{FindAnyObjectByType<PlayerMovement>().invincibilityFrames + threeUpgradeScreenReference.iFramesUpgradeIncrease}</color>\n";
                    break;
                case ThreeUpgradeScreen.StatIncreaseType.CastSpeed:
                    message = $"<u><color=yellow>Cast Speed Increase</color></u>\n\n{Mathf.Round(FindAnyObjectByType<PlayerMovement>().castSpeed * 100.00f) * 0.01f} -> <color=yellow>{Mathf.Round((FindAnyObjectByType<PlayerMovement>().castSpeed + threeUpgradeScreenReference.castSpeedUpgradeIncrease) * 100.00f) * 0.01f}</color>\n";
                    break;
                case ThreeUpgradeScreen.StatIncreaseType.CastStrength:
                    message = $"<u><color=yellow>Cast Strength Increase</color></u>\n\n{Mathf.Round(FindAnyObjectByType<PlayerMovement>().castStrength * 100.00f) * 0.01f} -> <color=yellow>{Mathf.Round((FindAnyObjectByType<PlayerMovement>().castStrength + threeUpgradeScreenReference.castStrengthUpgradeIncrease) * 100.00f) * 0.01f}</color>\n";
                    break;
                case ThreeUpgradeScreen.StatIncreaseType.DashStrength:
                    message = $"<u><color=yellow>Dash Strength Increase</color></u>\n\n{Mathf.Round(FindAnyObjectByType<PlayerMovement>().dashStrength * 100.00f) * 0.01f} -> <color=yellow>{Mathf.Round((FindAnyObjectByType<PlayerMovement>().dashStrength + threeUpgradeScreenReference.dashStrengthUpgradeIncrease) * 100.00f) * 0.01f}</color>\n";
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
