using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HatNodeUI : MonoBehaviour
{
    public GeneratedHat nodeData;
    [HideInInspector] public HatComponentManager hatVisuals;
    [SerializeField] private TextMeshProUGUI hatNodeName;
    [SerializeField] private Image nodeRarity;
    [SerializeField] private TextMeshProUGUI statLines;

    [Header("Hat Visuals")]
    [SerializeField] private Image prizeHatFront;
    [SerializeField] private Image prizeHatBack;
    [SerializeField] private Image prizeHatOutline;
    
    public void SetHat()
    {
        ConvertHatPrizeSpriteToUI(nodeData); // Sprite Visual
        hatNodeName.text = nodeData.hatName;
        nodeRarity.color = SetNodeColor();
        statLines.text = nodeData.PrintStatsOnly();
    }

    private Color SetNodeColor()
    {
        return nodeData.rarity switch
        {
            Rarity.Common => Color.gray,
            Rarity.Uncommon => Color.green,
            Rarity.Rare => Color.blue,
            Rarity.Epic => Color.magenta,
            Rarity.Legendary => Color.yellow,
            _ => Color.white,
        };
    }

    private void ConvertHatPrizeSpriteToUI(GeneratedHat hatData)
    {

        prizeHatFront.sprite = hatVisuals.ReturnSprite(hatVisuals.front);
        prizeHatFront.color = hatData.components.color;
        prizeHatBack.sprite = hatVisuals.ReturnSprite(hatVisuals.back);
        prizeHatOutline.sprite = hatVisuals.ReturnSprite(hatVisuals.outline);
        hatVisuals.DisableShadow();
    }
}
