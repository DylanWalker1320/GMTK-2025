using UnityEngine;

public class Stats : MonoBehaviour
{
    public GeneratedStat statData;

    public virtual void Initialize(GeneratedStat data, int index, bool applyStats = true)
    {
        statData = data;
    }
}
