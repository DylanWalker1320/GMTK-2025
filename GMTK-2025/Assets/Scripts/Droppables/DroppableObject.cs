using UnityEngine;

public class DroppableObject : MonoBehaviour
{
    public DropType dropType;
    public float powerValue;

    public enum DropType
    {
        ExperienceBoost,
        SoulBoost,
        LifeSteal,
        CastBoost,
        SpeedBoost,
        DashBoost,
        DropLength
    }
}
