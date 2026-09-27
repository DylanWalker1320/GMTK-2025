using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HatNodeUI : MonoBehaviour
{
    public GeneratedHat nodeData;
    public GameObject nodeVisuals;
    [SerializeField] private Transform nodeVisualPosition;
    [SerializeField] private TextMeshProUGUI hatNodeName;
    [SerializeField] private Image nodeRarity;
    [SerializeField] private TextMeshProUGUI statLines;
    
    public void SetHat()
    {
        nodeVisuals.transform.position = nodeVisualPosition.position;
        hatNodeName.text = nodeData.hatName;
        nodeRarity.color = SetNodeColor();
        statLines.text = nodeData.ToString();
    }

    private Color SetNodeColor()
    {
        switch(nodeData.rarity)
        {
            case Rarity.Common:
                return Color.gray;
            case Rarity.Uncommon:
                return Color.green;
            case Rarity.Rare:
                return Color.blue;
            case Rarity.Epic:
                return Color.magenta;
            case Rarity.Legendary:
                return Color.red;
            default:
                return Color.white;

        }
    }
}
