using UnityEngine;

public abstract class Spell : MonoBehaviour
{
    protected struct Type
    {
        public bool fire;
        public bool water;
        public bool lightning;
        public bool dark;
    }

    protected Type spellType;
    protected float speed;
    protected Vector2 direction;
}
