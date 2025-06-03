using Mirror;
using System.Collections;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    public float speed;

    [SyncVar]
    public float health;
    [SyncVar]
    public float maxHealth;
    [SyncVar(hook = nameof(OnSpriteTypeChanged))]
    public int spriteType;
    [SyncVar]
    public bool isLive;

    [SyncVar(hook = nameof(OnDeadStateChanged))]
    public bool isDead;

    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;
    WaitForFixedUpdate wait;

    [SyncVar(hook = nameof(OnEnemyFlipXChanged))]
    public bool currentFlipXState;

    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        wait = new WaitForFixedUpdate();
    }

    public override void OnStartServer()
    {
        if (isServer)
        {
            isLive = true;
            isDead = false;
            coll.enabled = true;
            rigid.simulated = true;
            spriter.sortingOrder = 2;
            anim.SetBool("Dead", false);
            health = maxHealth;
        }

        //if (isServer)
        //{
        //    Player nearestPlayer = GameManager.instance.GetNearestPlayer(transform.position); // 가장 가까운 플레이어
        //    if (nearestPlayer != null)
        //        target = nearestPlayer.GetComponent<Rigidbody2D>();
        //}
    }

    void FixedUpdate()
    {
        if (!isServer || !isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;

        if (isServer)
        {
            Player nearestPlayer = GameManager.instance.GetNearestPlayer(transform.position); // 가장 가까운 플레이어
            if (nearestPlayer != null)
                target = nearestPlayer.GetComponent<Rigidbody2D>();
        }


        if (target != null)
        {
            Vector2 dirVec = target.position - rigid.position;
            Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);
            rigid.linearVelocity = Vector2.zero;

            bool newFlipX = target.position.x < rigid.position.x;
            if (newFlipX != currentFlipXState)
            {
                currentFlipXState = newFlipX;
            }
        }
    }

    void OnEnemyFlipXChanged(bool oldFlipXState, bool newFlipXState)
    {
        spriter.flipX = newFlipXState;
    }

    void OnSpriteTypeChanged(int oldSpriteType, int newSpriteType)
    {
        if (anim != null && animCon.Length > newSpriteType && newSpriteType >= 0)
        {
            anim.runtimeAnimatorController = animCon[newSpriteType];
        }
    }

    void OnDeadStateChanged(bool oldValue, bool newValue)
    {
        if (anim != null)
        {
            anim.SetBool("Dead", newValue);
            coll.enabled = !newValue;
            rigid.simulated = !newValue;
            spriter.sortingOrder = newValue ? 1 : 2;
        }
    }

    [Server]
    public void Init(SpawnData data)
    {
        speed = data.speed;
        spriteType = data.spriteType;
        maxHealth = data.health;
        health = data.health;
        isLive = true;
        isDead = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive)
            return;

        if (!isServer) return;

        health -= collision.GetComponent<Bullet>().damage;
        StartCoroutine(KnockBack());

        if (health > 0)
        {
            RpcPlayHitAnim();
        }
        else
        {
            isLive = false;
            isDead = true;
            coll.enabled = false;
            rigid.simulated = false;
            StartCoroutine(DelayedDead());

            GameManager.instance.AddKill();
            GameManager.instance.AddExp();
        }
    }

    [Server]
    IEnumerator KnockBack()
    {
        yield return wait;

        Player nearestPlayer = GameManager.instance.GetNearestPlayer(transform.position); // 가장 가까운 플레이어
        Vector3 playerPos = nearestPlayer != null ? nearestPlayer.transform.position : transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
    }

    [ClientRpc]
    void RpcPlayHitAnim()
    {
        if (anim != null)
            anim.SetTrigger("Hit");
    }

    [Server]
    IEnumerator DelayedDead()
    {
        yield return new WaitForSeconds(1f); // 죽는 애니메이션 재생 시간

        Dead();
    }

    [Server]
    void Dead()
    {
        GameManager.instance.pool.ReturnToPool(gameObject);
    }
}
