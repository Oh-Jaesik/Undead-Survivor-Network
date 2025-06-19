using UnityEngine;
using Mirror;

public class Spawner : NetworkBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;

    int level;      // ���� ����
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

        timer += Time.deltaTime;        // ���� ���� ��Ÿ��
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / 20f), spawnData.Length - 1);

        if (timer > spawnData[level].spawnTime)     // ���� ����
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
        enemy.GetComponent<Enemy>().Init(spawnData[Random.Range(0,level+1)]);       // 0���� ���� �ð����� ���� �������� ���� ���� ����
    }
}

[System.Serializable]       // ����ȭ. Ŭ���� �ʱ�ȭ�� ���� �ν�����â���� Ŭ������ ����ü�� ���� ����
public class SpawnData
{
    public float spawnTime;
    public int spriteType;
    public int health;
    public float speed;
}
