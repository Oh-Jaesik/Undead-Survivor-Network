using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [Header("Health")]
    [SyncVar(hook = nameof(OnHealthChanged))]
    public float health;

    public float maxHealth = 100;

    [SyncVar(hook = nameof(OnInputVecChanged))]
    public Vector2 inputVec;

    [SyncVar(hook = nameof(OnFlipXChanged))]
    public bool isFlipped;

    public float speed;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;

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
        health = maxHealth; // 서버에서 체력 초기화
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

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        Vector2 currentInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (currentInput != inputVec)
        {
            CmdSetInputVec(currentInput);
        }

        bool currentFlipState = spriter.flipX;
        if (currentInput.x != 0)
        {
            currentFlipState = currentInput.x < 0;
        }

        if (currentFlipState != isFlipped)
        {
            CmdSetFlipState(currentFlipState);
        }
    }

    void FixedUpdate()
    {
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    [Command]
    void CmdSetInputVec(Vector2 newVec)
    {
        inputVec = newVec;
    }

    [Command]
    void CmdSetFlipState(bool newFlipState)
    {
        isFlipped = newFlipState;
    }

    void OnInputVecChanged(Vector2 oldInputVec, Vector2 newInputVec)
    {
        anim.SetFloat("Speed", newInputVec.magnitude);
    }

    void OnFlipXChanged(bool oldFlipState, bool newFlipState)
    {
        spriter.flipX = newFlipState;
    }

    void OnHealthChanged(float oldValue, float newValue)
    {
        // 체력 UI 갱신, 사망 처리
        if (newValue <= 0)
        {
            anim.SetTrigger("Dead");
            if (isLocalPlayer)
            {
                Debug.Log("You Died!");
                // GameOver UI 표시 등
            }
        }
    }

    [ServerCallback]
    void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.instance.isLive)
            return;

        if (health <= 0)
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
        GameManager.instance.GameOver(); // 확인 요

    }
}
