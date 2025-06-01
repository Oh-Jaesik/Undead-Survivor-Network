//using UnityEngine;
//using Mirror;
//using UnityEngine.UIElements;
//using UnityEngine.TextCore.Text;

//public class Weapon : NetworkBehaviour
//{
//    public int id;
//    public int prefabId;
//    public float damage;
//    public int count;
//    public float speed;

//    float timer;
//    Player player;

//    void Awake()
//    {
//        player = GameManager.instance.player;
//    }

//    public override void OnStartLocalPlayer()
//    {
//        GameManager.instance.weapon = this;

//    }


//    void Update()
//    {

//        Debug.Log("1");


//        //if (!isServer)
//        //{
//        //    return;
//        //}

//        switch (id)
//        {
//            case 0:
//                transform.Rotate(Vector3.back * speed * Time.deltaTime);
//                break;

//            default:

//                break;
//        }

//        if (Input.GetButtonDown("Jump"))
//        {
//            LevelUp(20, 5);
//        }
//    }


//    public void LevelUp(float damage, int count)
//    {
//        this.damage = damage;
//        this.count += count;

//        if (id == 0)
//            Batch();
//    }

//    [Server]
//    public void Init(ItemData data)
//    {

//        // Basic Set
//        name = "Weapon " + data.itemId;
//        //transform.parent = player.transform;
//        transform.localPosition = Vector3.zero;

//        // Property Set
//        id = data.itemId;
//        damage = data.baseDamage;
//        count = data.baseCount;

//        for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
//        {
//            if (data.projectile == GameManager.instance.pool.prefabs[index])
//            {
//                prefabId = index;
//                break;
//            }
//        }

//        switch (id)
//        {
//            case 0: // 근접 무기
//                speed = -150; // 회전 속도
//                Batch();      // 서버에서 삽 배치
//                break;
//            default: // 원거리 무기
//                speed = 0.3f; // 발사 딜레이
//                break;
//        }
//    }

//    [Server]
//    void Batch()
//    {
//        for (int index = 0; index < count; index++)
//        {
//            float angle = 360f * index / count;
//            Quaternion rot = Quaternion.Euler(0f, 0f, angle);
//            Vector3 offset = rot * Vector3.up * 1.5f;

//            GameObject bulletObj = GameManager.instance.pool.Get(prefabId, transform.position, Quaternion.identity);
//            Bullet bullet = bulletObj.GetComponent<Bullet>();

//            bullet.followTarget = transform; // weapon
//            bullet.offset = offset;          // 초기 위치 offset


//            Debug.Log("2");



//            bullet.Init(damage, -1, Vector3.zero);
//        }
//    }

//}


using UnityEngine;
using Mirror;
using Unity.VisualScripting;

public class Weapon : NetworkBehaviour
{
    public int id;
    public int prefabId;
    public float speed;

    float timer;
    Player player;

    [SyncVar(hook = nameof(OnDamageChanged))]
    public float damage;

    [SyncVar(hook = nameof(OnCountChanged))]
    public int count;

    void Awake()
    {
        player = GameManager.instance.player;
    }

    public override void OnStartLocalPlayer()
    {
        GameManager.instance.weapon = this;


      
    }

    public override void OnStartServer()
    {
        //Init();
    }

    void Update()
    {
        
        if (!isServer) return;

        if (Input.GetButtonDown("Jump"))
        {
            CmdLevelUp(20, 5);
        }

        if (id == 0)
        {
            transform.Rotate(Vector3.back * speed * Time.deltaTime);
        }
    }

    // === 서버에서만 실행되는 초기화 ===
    [Server]
    public void Init(ItemData data)
    {
        if (!isServer)
            return;

        name = "Weapon " + data.itemId;
        transform.localPosition = Vector3.zero;

        id = data.itemId;
        damage = data.baseDamage;
        count = data.baseCount;

        for (int i = 0; i < GameManager.instance.pool.prefabs.Length; i++)
        {
            if (data.projectile == GameManager.instance.pool.prefabs[i])
            {
                prefabId = i;
                break;
            }
        }

        switch (id)
        {
            case 0:
                speed = -150;
                Batch();
                break;
            default:
                speed = 0.3f;
                break;
        }
    }

    [Server]
    public void Init()
    {


        switch (id)
        {
            case 0:
                speed = -150;
                Batch();
                break;
            default:
                speed = 0.3f;
                break;
        }
    }

    // === 서버에서만 실행 ===
    [Command]
    public void CmdLevelUp(float newDamage, int addCount)
    {
        damage = newDamage;
        count += addCount;

        if (id == 0)
        {
            Batch();
        }
    }

    // === SyncVar 값이 바뀔 때 호출되는 Hook ===
    void OnDamageChanged(float oldValue, float newValue)
    {
        damage = newValue;
    }

    void OnCountChanged(int oldValue, int newValue)
    {
        count = newValue;
    }

    // === 서버에서 총알 배치 ===
    [Server]
    void Batch()
    {
        for (int i = 0; i < count; i++)
        {
            float angle = 360f * i / count;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);
            Vector3 offset = rot * Vector3.up * 1.5f;

            GameObject bulletObj = GameManager.instance.pool.Get(prefabId, transform.position, Quaternion.identity);
            Bullet bullet = bulletObj.GetComponent<Bullet>();

            bullet.followTarget = transform;
            bullet.offset = offset;

            bullet.Init(damage, -1, Vector3.zero);
        }
    }
}
