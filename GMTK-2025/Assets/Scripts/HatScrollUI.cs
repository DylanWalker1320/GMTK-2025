using UnityEngine;
using UnityEngine.Events;

public class HatScrollUI : MonoBehaviour
{
    [SerializeField] private GameObject scrollUI;
    [SerializeField] private GameObject scrollButton;
    [SerializeField] private GameObject hatPrizeUI;
    [SerializeField] private UnityEvent unityEvent;

    void Update()
    {
        if(hatPrizeUI.activeSelf && Input.GetKeyDown(KeyCode.Mouse0))
        {
            unityEvent.Invoke();
        }
    }

    public void ToggleScrollUI()
    {
        scrollUI.SetActive(!scrollUI.activeSelf);
        scrollButton.SetActive(!scrollButton.activeSelf);
    }

    public void ToggleHatPrize()
    {
        hatPrizeUI.SetActive(!hatPrizeUI.activeSelf);
    }
}
