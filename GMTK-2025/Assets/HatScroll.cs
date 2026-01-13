using System.Collections.Generic;
using UnityEngine;

public class HatScroll : MonoBehaviour
{
    [SerializeField]
    private GameObject _prefab;

    private float _speed;
    private bool _isScrolling;

    private List<HatCell> _cells = new List<HatCell>();

    public void Scroll()
    {
        if(_isScrolling)
            return;
        _speed = Random.Range(4, 5);
        _isScrolling = true;


        GetComponent<RectTransform>().localPosition = new Vector2(1080, 0);

        if (_cells.Count == 0)
        {
            for (int i = 0; i < 50; i++)
            {
                _cells.Add(Instantiate(_prefab, transform).GetComponentInChildren<HatCell>());
            }
        }
        foreach(var cell in _cells)
        {
            cell.Setup();
        }
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.left * 100, _speed * Time.deltaTime * 30); // Magic number, replace 30 with a variable

        if(_speed > 0)
        {
            _speed -= Time.deltaTime * Random.Range(1.2f, 1.5f); // Magic Numbers, replace with variables
        }
        else
        {
            _speed = 0;
            _isScrolling = false;
        }

    }
}
