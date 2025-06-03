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

    [Server]
    public void Init(ItemData data)
    {
        //if (!isServer)
        //    return;


        // Basic Set
        //name = "Weapon " + data.itemId;
        //transform.parent = player.transform;
        //transform.localPosition = Vector3.zero;


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
                speed = 150;
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
                speed = 150;
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

   
    void OnDamageChanged(float oldValue, float newValue)
    {
        damage = newValue;
    }

    void OnCountChanged(int oldValue, int newValue)
    {
        count = newValue;
    }

    
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
