using UnityEngine;
using Mirror;
using System;

public class Player : NetworkBehaviour
{
    [SyncVar]
    public float health;
    public float maxHealth;
    public float speed;
    public int statPoints;
    float timer;

    public Scanner scanner;
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    public Animator anim;
    public RuntimeAnimatorController[] animCon;

    [SyncVar(hook = nameof(OnInputVecChanged))]
    public Vector2 inputVec;
    [SyncVar(hook = nameof(OnAnimControllerIndexChanged))]
    public int animControllerIndex;

    void OnAnimControllerIndexChanged(int oldIndex, int newIndex)
    {
        anim.runtimeAnimatorController = animCon[newIndex];     // player 캐릭터 스프라이트 설정
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
    }

    public override void OnStartLocalPlayer()
    {
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = new Vector3(0, 0, -10);

        if (GameManager.instance != null && GameManager.instance.player == null)
        {
            GameManager.instance.player = this;
        }
    }

    public override void OnStartServer()
    {
        GameManager.instance.RegisterPlayer(this);
        health = maxHealth;
    }

    public override void OnStopServer()
    {
        GameManager.instance.UnregisterPlayer(this);
    }

    public override void OnStartClient()
    {
        if (GameManager.instance != null && GameManager.instance.players.Count == 0)
        {
            GameManager.instance.RegisterPlayer(this);
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (!GameManager.instance.isLive)
            return;

        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        CmdSetInputVec(moveInput);

        timer += Time.deltaTime;

        if (timer > GameManager.instance.weapon1.speed)
        {
            timer = 0f;
            CmdFire();      // 원거리 무기 발사
        }

        CmdChangeAnimController(animControllerIndex);
        anim.runtimeAnimatorController = animCon[animControllerIndex];
    }

    [Command]
    void CmdChangeAnimController(int index)
    {
        anim.runtimeAnimatorController = animCon[index];
    }

    [Command]
    void CmdSetInputVec(Vector2 moveInput)
    {
        inputVec = moveInput;
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    void OnInputVecChanged(Vector2 oldVec, Vector2 newVec)
    {
        anim.SetFloat("Speed", newVec.magnitude);

        if (newVec.x != 0)
            spriter.flipX = newVec.x < 0;
    }

    [Server]
    void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.instance.isLive || health <= 0)
            return;

        health -= Time.deltaTime * 10f;

        if (health <= 0)
        {
            health = 0;
            RpcDie();
        }
    }

    [ClientRpc]
    void RpcDie()
    {
        anim.SetTrigger("Dead");
        GameManager.instance.GameOver();
    }

    [Command]
    void CmdFire()
    {
        if (!scanner.nearestTarget)
            return;

        Vector3 targetPos = scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        GameObject bulletObj = GameManager.instance.pool.Get(2, transform.position, Quaternion.identity);
        Bullet bullet = bulletObj.GetComponent<Bullet>();

        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);       // 총알의 로컬기준 위방향을 dir로 설정

        bullet.followTarget = transform;

        bullet.GetComponent<Bullet>().Init(GameManager.instance.weapon1.damage, GameManager.instance.weapon1.count, dir);
    }
}