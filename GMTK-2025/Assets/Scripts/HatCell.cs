using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HatCell : MonoBehaviour
{
    private GameObject hatObject;
    private HatGenerator hatGenerator;
    private static List<GameObject> generatedHats;
    [SerializeField] private int[] _chances;

    public void Setup()
    {
        if (generatedHats == null)
            generatedHats = new List<GameObject>();

        hatGenerator = FindFirstObjectByType<HatGenerator>();
        hatObject = hatGenerator.GenerateHat();
        hatObject.transform.SetParent(this.transform);
        generatedHats.Add(hatObject);
        hatObject.transform.localPosition = Vector3.zero;

        // Reference the defined colours in the HatStats script
        GetComponent<Image>().color = RarityColors.Colors[GetHatRarity()];
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
