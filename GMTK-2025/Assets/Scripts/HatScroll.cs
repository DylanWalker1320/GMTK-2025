using System.Collections.Generic;
using UnityEngine;
public class HatScroll : MonoBehaviour
{
    [SerializeField]
    private GameObject _prefab;
    private GameObject _currentHatPrefabContainer;

    public float _speed;
    private bool _isScrolling;
    public bool _hasScrolled = false;
    private int counter;
    private List<HatCell> _cells = new List<HatCell>();
    private GameObject targetHatObject;
    private GeneratedHat targetHatData;
    private HatGenerator hatGenerator;

    public void Initialize()
    {
        _currentHatPrefabContainer = _prefab;
        GetComponent<RectTransform>().localPosition = new Vector2(1080, 0);

    }

    public void Scroll()
    {
        if (_isScrolling)
            return;
        _speed = Random.Range(4, 5);
        _isScrolling = true;
        counter = 0;



        GetComponent<RectTransform>().localPosition = new Vector2(1080, 0);

        if (_cells.Count == 0)
        {
            for (int i = 0; i < 50; i++)
            {
                _cells.Add(Instantiate(_currentHatPrefabContainer, transform).GetComponentInChildren<HatCell>());
            }
        }
        foreach (var cell in _cells)
        {
            counter++;
            cell.Setup();
            if (counter == 39)
            {
                targetHatObject = cell.GetHatObject();
                targetHatData = cell.GetHatData();
            }
        }
    }
    
    private void Start()
    {
        hatGenerator = FindFirstObjectByType<HatGenerator>();
    }

    private void Update() // With this setup, cell 39/50 will always win
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.left * 100, _speed * Time.unscaledDeltaTime * 30); // Magic number, replace 30 with a variable

        if (_speed > 0)
        {
            _speed -= Time.unscaledDeltaTime * 1.5f; // Magic Numbers, replace with variables
        }
        else if (_speed < 0 && _isScrolling)
        {
            _speed = 0;
            _isScrolling = false;
            _hasScrolled = true;

        }

    }

    public GeneratedHat GetTargetHatData()
    {
        return targetHatData;
    }
    
    public void ApplyPrizeHatStats()
    {
        hatGenerator.GeneratePlayerHatWithStats(targetHatData);
    }

}
