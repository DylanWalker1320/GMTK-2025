using UnityEngine;

public class Hat : MonoBehaviour
{
    public GeneratedHat hatData;
    
    // Called by HatGenerator after instantiation
    public virtual void Initialize(GeneratedHat data, bool applyStats = true)
    {
        hatData = data;
    }
}