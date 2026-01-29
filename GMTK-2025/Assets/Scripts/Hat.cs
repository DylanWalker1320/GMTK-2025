using UnityEngine;

[RequireComponent(typeof(HatSpriteLayerUpdater))]
public class Hat : MonoBehaviour
{
    public GeneratedHat hatData;
    public int hatSpriteMinLayer;
    
    // Called by HatGenerator after instantiation
    public virtual void Initialize(GeneratedHat data, bool applyStats = true)
    {
        hatData = data;
        //hatSpriteLayerUpdater = GetComponent<HatSpriteLayerUpdater>();
        //hatSpriteLayerUpdater.UpdateSpriteLayers();
    }
}