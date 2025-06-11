using UnityEngine;
using Mirror;

public class Spawner : NetworkBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;

    int level;      // 몬스터 레벨
    float timer;

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    [Server]
    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        timer += Time.deltaTime;        // 몬스터 스폰 쿨타임
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / 10f), spawnData.Length - 1);

        if (timer > spawnData[level].spawnTime)     // 스폰 조건
        {
            timer = 0;
            Spawn();
        }
    }

    [Server]
    void Spawn()
    {
        GameObject enemy = GameManager.instance.pool.Get(0, spawnPoint[Random.Range(1, spawnPoint.Length)].position, Quaternion.identity);
        enemy.transform.parent = GameManager.instance.pool.transform;
        enemy.GetComponent<Enemy>().Init(spawnData[Random.Range(0,level+1)]);       // 0부터 현재 시간기준 몬스터 레벨까지 몬스터 랜덤 생성
    }
}

[System.Serializable]       // 직렬화. 클래스 초기화를 통해 인스펙터창에서 클래스나 구조체에 접근 가능
public class SpawnData
{
    public float spawnTime;
    public int spriteType;
    public int health;
    public float speed;
}
