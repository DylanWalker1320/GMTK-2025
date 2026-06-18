using UnityEngine;

public class HermitProjectile : MonoBehaviour
{
    [SerializeField] private Vector2 startPosition;
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private Transform visual;  // Assign your sprite/mesh here
    [SerializeField] private GameObject targetMarkerPrefab;
    [SerializeField] private float peakOffset = 2f;      // k: units above the higher endpoint
    [SerializeField] private float speedPerUnit = 1f;    // seconds per unit of distance

    private float _t = 0f;
    private float _duration;

    // Parabola parameters (computed in 1D along the flight path)
    private float _totalDist;   // x2 in parabola space (x1 = 0)
    private float _v;           // vertex height
    private float _h;           // vertex x in parabola space
    private float _a;           // parabola coefficient

    // y values in parabola space: start=0, end=0 since we offset by startHeight
    private float _startHeight; // world height of start (always 0 in your case, but kept for generality)
    private float _endHeight;   // world height of target relative to start

    void Start()
    {
        startPosition = transform.position;
        Instantiate(targetMarkerPrefab, targetPosition, Quaternion.identity);

        targetPosition = GameObject.FindWithTag("Player").transform.position;

        _totalDist = Vector2.Distance(startPosition, targetPosition);
        _duration = _totalDist * speedPerUnit;

        // In parabola space: x1 = 0, y1 = 0 (start), x2 = totalDist, y2 = endHeight
        _startHeight = 0f;
        _endHeight = 0f;  // both endpoints at ground level

        float y1 = _startHeight;
        float y2 = _endHeight;
        float x1 = 0f;
        float x2 = _totalDist;

        _v = Mathf.Max(y1, y2) + peakOffset;

        float r = Mathf.Sqrt((y1 - _v) / (y2 - _v));  // both endpoints below v so this is real
        _h = (x1 + r * x2) / (1f + r);                // weighted average form (always in-range)

        _a = (y1 - _v) / Mathf.Pow(x1 - _h, 2);
    }

    void Update()
    {
        if (_t >= 1f) return;

        _t += Time.deltaTime / _duration;
        _t = Mathf.Clamp01(_t);

        // World XZ position: lerp along the top-down direction
        Vector2 worldPos = Vector2.Lerp(startPosition, targetPosition, _t);

        // Parabola space x: how far along the flight path are we
        float parabolaX = _t * _totalDist;

        // Visual height from parabola
        float height = _a * Mathf.Pow(parabolaX - _h, 2f) + _v;

        // Apply: XY is top-down position
        transform.position = new Vector3(worldPos.x, worldPos.y, 0f);
        ApplyVisualHeight(height);

        if (_t >= 1f) OnArrival();
    }

    private void ApplyVisualHeight(float height)
    {
        // Offset the visual child object upward in screen space
        if (visual != null)
            visual.localPosition = new Vector3(0f, height, 0f);
    }

    private void OnArrival()
    {
        // Destroy, trigger impact, etc.
        Destroy(gameObject);
    }
}