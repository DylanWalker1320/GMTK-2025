using UnityEngine;

public abstract class Hat : MonoBehaviour
{
    protected GeneratedHat hatData;
    
    // Called by HatGenerator after instantiation
    public abstract void Initialize(GeneratedHat data);
}