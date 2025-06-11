using UnityEngine;
using Mirror;
using Unity.VisualScripting;
using System;
using UnityEngine.UIElements;

public class Weapon : NetworkBehaviour
{
    public int id;
    public int prefabId;

    [SyncVar(hook = nameof(OnDamageChanged))]
    public float damage;

    [SyncVar(hook = nameof(OnCountChanged))]
    public int count;

    [SyncVar(hook = nameof(OnSpeedChanged))]
    public float speed;

    void OnDamageChanged(float oldValue, float newValue)
    {
        damage = newValue;
    }

    void OnCountChanged(int oldValue, int newValue)
    {
        count = newValue;
    }

    void OnSpeedChanged(float oldValue, float newValue)
    {
        speed = newValue;
    }

    public override void OnStartLocalPlayer()
    {
        if (id == 0)
            GameManager.instance.weapon = this;
        if (id == 1)
            GameManager.instance.weapon1 = this;
    }

    void Update()
    {
        if (!isServer)
            return;

        if (!GameManager.instance.isLive)
            return;

        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;

        }
    }

    public void Init(ItemData data)
    {
        // Basic Set
        //name = "Weapon " + data.itemId;
        //transform.parent = player.transform;
        //transform.localPosition = Vector3.zero;

        id = data.itemId;
        damage = data.baseDamage;
        count = data.baseCount;

        switch (id)
        {
            case 0:
                speed = 150;
                Batch();
                break;
            default:
                speed = 1f;
                break;
        }
    }

    [Command]
    public void CmdLevelUp(float newDamage, int addCount)
    {
        // ScriptObj의 damage, count 값을 그대로 읽어오도록 수정
        damage = newDamage;
        count = addCount;

        // speed 공식은 Gear코드 참고
        switch (id)
        {
            case 0:
                speed = 150 + 500 * GameManager.instance.gear0.rate;
                Batch();
                break;
            case 1:
                speed = 1f - 2 * GameManager.instance.gear0.rate;
                break;
        }
    }

    [Server]
    void Batch()
    {
        //기존 Bullet 0 제거
        foreach (var bullet in FindObjectsOfType<Bullet>())
        {
            if (bullet.ownerWeapon == this && bullet.name.StartsWith("Bullet 0")) // 이름이 "Bullet 0"으로 시작하는 것만 삭제
            {
                NetworkServer.UnSpawn(bullet.gameObject);
                Destroy(bullet.gameObject);
            }
        }


        for (int i = 0; i < count; i++)
        {
            float angle = 360f * i / count;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);
            Vector3 offset = rot * Vector3.up * 1.5f;

            //GameObject bulletObj = GameManager.instance.pool.Get(prefabId, transform.position, Quaternion.identity);

            GameObject bulletObj = Instantiate(GameManager.instance.pool.prefabs[prefabId], transform.position, Quaternion.identity);
            NetworkServer.Spawn(bulletObj);


            Bullet bullet = bulletObj.GetComponent<Bullet>();

            bullet.ownerWeapon = this;      // bullet 주인 누구인지 (오브젝트 상속 대체)
            bullet.followTarget = transform;
            bullet.offset = offset;

            bullet.Init(damage, -1, Vector3.zero);
        }
    }
}
