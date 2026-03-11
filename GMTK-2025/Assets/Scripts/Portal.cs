using UnityEngine;
using Unity.Cinemachine;

public class Portal : MonoBehaviour
{
    public Transform location = null;
    private static CinemachineFollow camera;

    void Start()
    {
        if (camera == null)
        {
            camera = GameObject.FindGameObjectWithTag("MainCamera").transform.parent.gameObject.GetComponent<CinemachineFollow>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            camera.OnTargetObjectWarped(other.transform, location.position - other.transform.position);
            other.transform.position = location.position;
        }
    }
}
