using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
{
    public float damage;
    public int per;

    public Transform followTarget;  
    public Vector3 offset;         
    Rigidbody2D rigid;
    public Weapon ownerWeapon;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        if (per == -100 && followTarget != null)
        {
            transform.position = followTarget.position + followTarget.rotation * offset;
        }
    }

    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;

        if (per > -1)
        {
            rigid.linearVelocity = dir * 15f;
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") || per == -100)
            return;

        per--;

        if (per < 0)
        {
            rigid.linearVelocity = Vector2.zero;
            //gameObject.SetActive(false);
            GameManager.instance.pool.ReturnToPool(gameObject);     // network unspawn
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || per == -100)
            return;

        GameManager.instance.pool.ReturnToPool(gameObject);
    }
}