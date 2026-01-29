using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HatCell : MonoBehaviour
{
    [System.Serializable]
    private class ListOfSprites
    {
        public List<Sprite> HatSprites;
    }

    private GameObject hatObject;
    private HatGenerator hatGenerator;
    private int colorIndex;
    [SerializeField] private List<ListOfSprites> _sprites;

    [SerializeField] private int[] _chances;
    [SerializeField] private Color[] _colors;

    public void Setup()
    {
        hatGenerator = FindFirstObjectByType<HatGenerator>();
        hatObject = hatGenerator.GenerateHat();
        hatObject.transform.SetParent(this.transform);
        colorIndex = (int)GetHatRarity();
        transform.parent.GetComponent<Image>().color = _colors[colorIndex];
 
    }

    public Rarity GetHatRarity()
    {
        return GetHatData().rarity;
    }


    public GeneratedHat GetHatData()
    {
        return hatObject.GetComponent<Hat>().hatData;
    }
    public GameObject GetHatObject()
    {
        return hatObject;
    }
}
