using UnityEngine;
using System.Collections;

public abstract class Spell : MonoBehaviour
{
    protected enum Type
    {
        Fire,
        Water,
        Lightning,
        Dark,
        None
    }

    [Header("Spell Properties")]
    [SerializeField] protected float destroyTime = 5f;
    [SerializeField] protected float damage = 2f; // Damage dealt by the spell
    [SerializeField] protected float speed;
    [SerializeField] protected Type spellType1;
    [SerializeField] protected Type spellType2;
    protected Rigidbody2D rb;
    protected Vector3 mousePos;
    protected Camera mainCam;
    
    protected void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    protected void OrientSpell()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - transform.position;
        Vector3 rotation = transform.position - mousePos;
        rb.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(direction.x, direction.y).normalized * speed; //normalized so that ball stays at a constant speed no matter how far mouse is from player
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg; //make a degree float
        transform.rotation = Quaternion.Euler(0, 0, rot - 180);
    }
    
    protected IEnumerator DestroyAfter(float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        Destroy(gameObject);
    }
}