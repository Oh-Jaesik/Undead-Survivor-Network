using UnityEngine;
using Mirror;

public class Spawner : NetworkBehaviour        // player 밑에 spawner 오브젝트 붙어있음. 그밑에 spawn point 오브젝트 붙어있음.
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;

    int level;
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
        //enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;     // player 위치 빼고 1부터 시작
        enemy.transform.parent = GameManager.instance.pool.transform;
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
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
