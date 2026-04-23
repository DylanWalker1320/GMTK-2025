using UnityEngine;
using Unity.Cinemachine;
public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance {get; private set;}
    private CinemachineCamera cineMachineCamera;
    private float shakeTimer;

    void Awake()
    {
        Instance = this;
        cineMachineCamera = GetComponent<CinemachineCamera>();
    }

    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cineMachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.AmplitudeGain = intensity;
        shakeTimer = time;

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if(shakeTimer <= 0f)
            {
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cineMachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();

                cinemachineBasicMultiChannelPerlin.AmplitudeGain = 0f;
            }
        }
    }
}
