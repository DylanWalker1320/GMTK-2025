using UnityEngine;

public class Stats : MonoBehaviour
{
    public GeneratedStat statData;

    public virtual void Initialize(GeneratedStat data, bool applyStats = true)
    {
        statData = data;
    }
}
