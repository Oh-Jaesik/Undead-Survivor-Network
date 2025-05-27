using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
{
    public float damage;
    public int per;

    public Transform followTarget;  
    public Vector3 offset;         

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        if (per == -1 && followTarget != null)
        {
            transform.position = followTarget.position + followTarget.rotation * offset;
            //transform.rotation = followTarget.rotation;
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
}