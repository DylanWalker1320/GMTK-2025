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

    [SerializeField] private List<ListOfSprites> _sprites;

    [SerializeField] private int[] _chances;
    [SerializeField] private Color[] _colors;

    public void Setup()
    {
        var index = Randomize();
        Debug.Log(index);
        GetComponent<Image>().sprite = _sprites[index].HatSprites[Random.Range(0, _sprites[index].HatSprites.Count)];
        transform.parent.GetComponent<Image>().color = _colors[index];
    }

    private int Randomize()
    {
        int index = 0;
        for (int i = 0; i < _chances.Length; i++)
        {
            int rand;
            if(i + 1 == _chances.Length)
            {
                rand = 100;
            }
            else
            {
                rand = Random.Range(0, 100);   
            }
            if (rand > _chances[i])
            {
                index = i;
                break;
            }
            index++;
        }
        return index;
    }
}
