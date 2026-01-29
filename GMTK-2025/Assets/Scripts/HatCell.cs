using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HatCell : MonoBehaviour
{
    private GameObject hatObject;
    private HatGenerator hatGenerator;
    private static List<GameObject> generatedHats;
    [SerializeField] private int[] _chances;

    public void Start()
    {
        generatedHats = new List<GameObject>();
    }

    public void Setup()
    {
        hatGenerator = FindFirstObjectByType<HatGenerator>();
        hatObject = hatGenerator.GenerateHat();
        hatObject.transform.SetParent(this.transform);
        generatedHats.Add(hatObject);

        // Set the border color based on rarity
        transform.parent.GetComponent<Image>().color = RarityColors.Colors[GetHatRarity()];
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

    public static void ClearGeneratedHats()
    {
        foreach (var hat in generatedHats)
        {
            GameObject.Destroy(hat);
        }
        generatedHats.Clear();
    }
}
