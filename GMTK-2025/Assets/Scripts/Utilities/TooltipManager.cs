using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{

    public static TooltipManager _instance;
    public TextMeshProUGUI tooltipText;
    private new Camera camera;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = true;
        camera = Camera.main;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 worldSpacePosition = camera.ScreenToWorldPoint(Input.mousePosition);
        worldSpacePosition.z = 0;
        transform.position = worldSpacePosition;
    }

    public void SetAndShowTooltip(string message)
    {
        
        gameObject.SetActive(true);
        tooltipText.text = message;
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
        tooltipText.text = string.Empty;
    }


}
