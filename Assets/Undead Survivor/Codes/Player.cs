//using UnityEngine;
//using Mirror;

//public class Player : NetworkBehaviour
//{
//    //public override void OnStartServer()
//    //{
//    //    if (GameManager.instance != null && GameManager.instance.player == null)
//    //    {
//    //        GameManager.instance.player = this;
//    //    }
//    //}


//    public override void OnStartClient()
//    {
//        if (isServer)
//        {
//            if (GameManager.instance != null && GameManager.instance.player == null)
//            {
//                GameManager.instance.player = this;
//            }
//        }
//    }



//    [SyncVar(hook = nameof(OnInputVecChanged))]
//    public Vector2 inputVec;

//    [SyncVar(hook = nameof(OnFlipXChanged))]
//    public bool isFlipped;

//    public float speed;

//    Rigidbody2D rigid;
//    SpriteRenderer spriter;
//    Animator anim;

//    void Awake()
//    {
//        rigid = GetComponent<Rigidbody2D>();
//        spriter = GetComponent<SpriteRenderer>();
//        anim = GetComponent<Animator>();
//    }

//    void Update()
//    {
//        if (!isLocalPlayer)
//        {
//            return;
//        }

//        Vector2 currentInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

//        if (currentInput != inputVec)
//        {
//            CmdSetInputVec(currentInput);
//        }

//        bool currentFlipState = spriter.flipX;
//        if (currentInput.x != 0)
//        {
//            currentFlipState = currentInput.x < 0;
//        }

//        if (currentFlipState != isFlipped)
//        {
//            CmdSetFlipState(currentFlipState);
//        }
//    }

//    void FixedUpdate()
//    {
//        // Player의 이동은 NetworkTransform에 의해 동기화된다고 가정합니다.
//        // 클라이언트에서 inputVec에 따라 rigid.MovePosition을 호출하고,
//        // NetworkTransform이 이를 서버로 전송하고 다른 클라이언트로 동기화합니다.
//        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
//        rigid.MovePosition(rigid.position + nextVec);
//    }

//    [Command]
//    void CmdSetInputVec(Vector2 newVec)
//    {
//        inputVec = newVec;
//    }

//    [Command]
//    void CmdSetFlipState(bool newFlipState)
//    {
//        isFlipped = newFlipState;
//    }

//    void OnInputVecChanged(Vector2 oldInputVec, Vector2 newInputVec)
//    {
//        anim.SetFloat("Speed", newInputVec.magnitude);
//    }

//    void OnFlipXChanged(bool oldFlipState, bool newFlipState)
//    {
//        spriter.flipX = newFlipState;
//    }
//}


using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    public override void OnStartServer()
    {
        GameManager.instance.RegisterPlayer(this);
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

    [SyncVar(hook = nameof(OnInputVecChanged))]
    public Vector2 inputVec;

    [SyncVar(hook = nameof(OnFlipXChanged))]
    public bool isFlipped;

    public float speed;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

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
}
