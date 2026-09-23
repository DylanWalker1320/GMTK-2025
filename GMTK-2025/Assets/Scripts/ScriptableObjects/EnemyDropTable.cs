using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDropTable", menuName = "Scriptable Objects/EnemyDropTable")]
public class EnemyDropTable : ScriptableObject
{
    public List<DropEntry> drops;
}

[System.Serializable]
public class DropEntry
{
    public DroppableObject dropObject;
    [Range(0, 1f)]
    public float probability;
}
