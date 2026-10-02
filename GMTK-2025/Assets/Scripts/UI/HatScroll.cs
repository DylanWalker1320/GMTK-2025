using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class HatScroll : MonoBehaviour
{
    public bool debugMode;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform viewport;
    [SerializeField] private float duration = 4.3f;
    [SerializeField] private float settlePrizeAt = 0.9f;
    private float easeOutFactor = 4;
    private GameObject _currentHatPrefabContainer;
    private bool _isFinished = false;
    private int counter;
    private int targetCellIndex = 30; // 0 based index -> 29
    private List<HatCell> _cells = new List<HatCell>();
    private GameObject targetHatObject;
    private HatScrollUI hatScrollUI;
    private GeneratedHat targetHatData;
    private HatGenerator hatGenerator;
    private Vector2 start;
    private Vector2 end;

    public void Initialize()
    {
        _isFinished = false;
        _currentHatPrefabContainer = _prefab;
        content = GetComponent<RectTransform>();
        hatScrollUI = FindAnyObjectByType<HatScrollUI>();
    }

    public void Scroll()
    {
        FindFirstObjectByType<UIManager>().scrollUI.GetComponent<Animator>().SetTrigger("HatRollRolling");
        FindFirstObjectByType<AudioManager>().Play("HATROLL");
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
            if (counter == targetCellIndex)
            {
                targetHatObject = cell.GetHatObject();
                targetHatData = cell.GetHatData();
                if (debugMode) Debug.Log("Target Hat Set to Cell 29 with Rarity: " + targetHatData);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        Canvas.ForceUpdateCanvases(); // Force the layout to update before calculating positions

        var winningCell = _cells[targetCellIndex - 1].gameObject.GetComponent<RectTransform>();
        // Both centers of winning cell and starting point in viewport = world space -> converted into content's local units
        Vector3 winningCellCenter = winningCell.TransformPoint(winningCell.rect.center);
        Vector3 viewportCenter = viewport.TransformPoint(viewport.rect.center);
        float deltaInverseX = content.InverseTransformVector(viewportCenter - winningCellCenter).x;

        // Start and End for scroll direction
        start = content.anchoredPosition;
        end = start + new Vector2(deltaInverseX, 0f);
        StartCoroutine(Roll());
    }
    
    private void Start()
    {
        hatGenerator = FindFirstObjectByType<HatGenerator>();
    }

    public IEnumerator Roll()
    {
        for (float time = 0; time < duration; time += Time.unscaledDeltaTime)
        {
            float timeOverDuration = time / duration;
            float t = 1f - Mathf.Pow(1f - timeOverDuration, easeOutFactor); // Ease out with decaying speed
            content.anchoredPosition = Vector2.LerpUnclamped(start, end, t);
            if(timeOverDuration >= settlePrizeAt & !_isFinished || ((Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.JoystickButton1)) && timeOverDuration > 0.05f))
            {
                _isFinished = true;
                hatScrollUI.ScrollCompleted();
            }
            yield return null;
        }
        
        content.anchoredPosition = end;
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

}
