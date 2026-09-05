using System.Collections.Generic;
using UnityEngine;
public class HatScroll : MonoBehaviour
{
    [SerializeField]
    private GameObject _prefab;
    private GameObject _currentHatPrefabContainer;

    public float _speed;
    public bool _hasScrolled = false;
    public float zeroTimerLength;
    private float zeroTimerOnFinish;
    private bool _isScrolling;
    private bool _hasInteracted;
    private int counter;
    private List<HatCell> _cells = new List<HatCell>();
    private GameObject targetHatObject;
    private GeneratedHat targetHatData;
    private HatGenerator hatGenerator;
    public bool debugMode;

    public void Initialize()
    {
        _currentHatPrefabContainer = _prefab;
        zeroTimerOnFinish = zeroTimerLength;
        _hasInteracted = false;
    }

    public void Scroll()
    {
        if (_isScrolling)
            return;
        FindFirstObjectByType<UIManager>().scrollUI.GetComponent<Animator>().SetTrigger("HatRollRolling");
        _speed = Random.Range(4, 5);
        _hasInteracted = true;
        _isScrolling = true;
        counter = 0;
        
        if (_cells.Count == 0)
        {
            for (int i = 0; i < 53; i++)
            {
                _cells.Add(Instantiate(_currentHatPrefabContainer, transform).GetComponentInChildren<HatCell>());
            }
        }
        foreach (var cell in _cells)
        {
            counter++;
            cell.Setup();
            if (counter == 29)
            {
                targetHatObject = cell.GetHatObject();
                targetHatData = cell.GetHatData();
                if (debugMode) Debug.Log("Target Hat Set to Cell 29 with Rarity: " + targetHatData);
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
            _speed -= Time.unscaledDeltaTime * 1.2f; // Magic Numbers, replace with variables
        }
        else if (_speed < 0.5f && _speed > 0)
        {
            _speed -= Time.unscaledDeltaTime * 3; // Magic Numbers, replace with variables
        }
        else if (_speed < 0 && _isScrolling)
        {
            _speed = 0;
            _isScrolling = false;
            if (debugMode) Debug.Log("Scrolling finished.");
        }
        else if(_speed == 0 && !_isScrolling && _hasInteracted)
        {
            zeroTimerOnFinish -= Time.unscaledDeltaTime;
            if (zeroTimerOnFinish <= 0)
            {
                _hasScrolled = true;
                if (debugMode) Debug.Log("Zero Timer finished, ready for UI Switch.");
            }
        }

    }

    public GeneratedHat GetTargetHatData()
    {
        return targetHatData;
    }
    
    public void ApplyPrizeHatStats()
    {
        hatGenerator.GeneratePlayerHatWithStats(targetHatData);

        // Clear generated hats to prevent memory leak
        if (debugMode) Debug.Log("<color=#55AAFF>[HatScroll]</color> Clearing generated hats to prevent memory leak...");
        HatCell.ClearGeneratedHats();
        HatScrollUI.DestroyTargetHat();
    }

    public bool GetIsScrolling()
    {
        return _isScrolling;
    }

    public void SetIsScrolling(bool setter)
    {
        _isScrolling = setter;
    }

}
