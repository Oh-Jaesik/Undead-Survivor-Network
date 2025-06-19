using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    [Header("# Game Control")]

    [SyncVar]
    public bool isLive;        // SyncVar ����, ���� �÷��̾� ���·θ� ���
    [SyncVar]
    public float gameTime;
    public float maxGameTime;
    public float bestTime;

    [Header("# Player Info (shared)")]

    public int playerId;
    [SyncVar(hook = nameof(OnLevelChanged))]
    public int level;
    [SyncVar]
    public int kill;
    [SyncVar]
    public int maxKill = 0;
    [SyncVar]
    public int exp;
    public int[] nextExp;

    [Header("# Game Object")]

    public PoolManager pool;
    public Player player;
    public List<Player> players = new List<Player>();
    public Result uiResult;
    public GameObject enemyCleaner;

    [Header("# Player Weapon")]
    public Weapon weapon;
    public Weapon weapon1;
    public Gear gear0;
    public Gear gear1;


    void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // �ߺ� ����
            return;
        }

        instance = this;
    }
    void Start()
    {
        FirebaseManager.Instance.LoadMaxKill();
        FirebaseManager.Instance.LoadBestTime();
    }



    // �÷��̾ ���� �ɷ� ����! case 2, 3�� Item �ڵ忡!
    public void GameStart(int id)
    {
        playerId = id;
        maxKill = SessionData.maxKill;
        bestTime = SessionData.bestTime;
        switch (id)
        {
            case 0:
                player.maxHealth += 20;
                player.health += 20;
                break;

            case 1:
                player.speed += 1;
                break;
        }


        player.health = player.maxHealth;
        player.gameObject.SetActive(true);

        player.animControllerIndex = id;

        enemyCleaner.SetActive(false);

        Resume();

        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;

        int finalKill = kill;             // ?? Ŭ���ʷ� ���� �߰� ų ������ ������
        float finalTime = gameTime;

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();     // �۵����ؼ� ���� �����ص�. �α��� �ڵ� �̻� �߻� ����??


        Debug.Log("?? FirebaseManager.Instance: " + (FirebaseManager.Instance != null));
        Debug.Log("?? FirebaseManager.dbRef: " + (FirebaseManager.Instance?.dbRef != null));

        yield return new WaitUntil(() =>
        {
            return FirebaseManager.Instance != null && FirebaseManager.Instance.dbRef != null;
        });//firebase �ʱ�ȭ

        if (finalKill > SessionData.maxKill)
            SessionData.maxKill = finalKill;

        if (finalTime > SessionData.bestTime)
            SessionData.bestTime = finalTime;

        FirebaseManager.Instance.SaveUserData();//������ ����
        Stop();
    }


    public void GameVictory()
    {
        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
        StartCoroutine(GameVictoryRoutine());
    }

    IEnumerator GameVictoryRoutine()
    {
        isLive = false;

        int finalKill = kill;             // ?? Ŭ���� �۵� �� ų �� ����
        float finalTime = gameTime;

        enemyCleaner.SetActive(true);     // ���� ��ü ����

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Win();

        yield return new WaitUntil(() =>
            FirebaseManager.Instance != null &&
            FirebaseManager.Instance.dbRef != null
        );

        if (finalKill > SessionData.maxKill)
            SessionData.maxKill = finalKill;

        if (finalTime > SessionData.bestTime)
            SessionData.bestTime = finalTime;

        FirebaseManager.Instance.SaveUserData();
        Stop();
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);      // ���� Ȯ�� ��
    }




    [Server]
    void Update()
    {
        if (!isLive)
            return;

        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;

            RpcVictory();
        }
    }

    [ClientRpc]
    void RpcVictory()
    {
        GameVictory();
    }

    [Server]
    public void AddKill()
    {
        kill++;
    }

    [Server]
    public void AddExp()
    {
        if (!isLive)
            return;

        exp++;

        int requiredExp = nextExp[Mathf.Min(level, nextExp.Length - 1)];

        if (exp >= requiredExp)
        {
            exp = 0;
            level++;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }


    void OnLevelChanged(int oldLevel, int newLevel)
    {
        GameManager.instance.player.statPoints++; // ���� ������ŭ ���� ȹ��
    }


    [Server]
    public void RegisterPlayer(Player player)
    {
        if (!players.Contains(player))
            players.Add(player);
    }

    [Server]
    public void UnregisterPlayer(Player player)
    {
        if (players.Contains(player))
            players.Remove(player);
    }

    public Player GetNearestPlayer(Vector3 fromPosition)
    {
        Player nearest = null;
        float minDist = float.MaxValue;

        foreach (Player p in players)
        {
            float dist = Vector3.Distance(fromPosition, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = p;
            }
        }

        return nearest;
    }
}