using UnityEngine;

public class HatTest : MonoBehaviour
{
    public GeneratedHat testHatData;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            HatGenerator hatGenerator = FindFirstObjectByType<HatGenerator>();
            hatGenerator.GeneratePlayerHatWithStats(testHatData, true);
        }
    }
}
