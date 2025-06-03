using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [Header("Health")]
    [SyncVar]
    public float health;

    public float maxHealth = 100;

    [SyncVar(hook = nameof(OnInputVecChanged))]
    public Vector2 inputVec;

    [SyncVar(hook = nameof(OnFlipXChanged))]
    public bool isFlipped;

    public float speed;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    public Animator anim;

    public RuntimeAnimatorController[] animCon;

    [SyncVar(hook = nameof(OnAnimControllerIndexChanged))]
    public int animControllerIndex;

    public int statPoints;



    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
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
        if (!isLocalPlayer) return;

        Vector2 currentInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // 로컬에서만 방향 전환 감지
        if (currentInput != inputVec)
        {
            bool shouldFlip = currentInput.x < 0;
            CmdSetMoveInput(currentInput, shouldFlip);
        }
    }

    void FixedUpdate()
    {
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    [Command]
    void CmdSetMoveInput(Vector2 moveInput, bool flip)
    {
        inputVec = moveInput;
        isFlipped = flip;
    }

    void OnInputVecChanged(Vector2 oldVec, Vector2 newVec)
    {
        anim.SetFloat("Speed", newVec.magnitude);
    }

    void OnFlipXChanged(bool oldFlip, bool newFlip)
    {
        spriter.flipX = newFlip;
    }

    void OnAnimControllerIndexChanged(int oldIndex, int newIndex)
    {
        anim.runtimeAnimatorController = animCon[newIndex];
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
}
