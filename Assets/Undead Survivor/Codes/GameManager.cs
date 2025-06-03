using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    [Header("# Game Control")]
    public bool isLive;        // SyncVar 없이, 로컬 플레이어 상태로만 사용
    [SyncVar]
    public float gameTime;
    public float maxGameTime = 2 * 10f;

    [Header("# Player Info (shared)")]
    public int playerId;
    public float health;
    public float maxHealth = 100;

    [SyncVar(hook = nameof(OnLevelChanged))]
    public int level;

    [SyncVar(hook = nameof(OnKillChanged))]
    public int kill;

    [SyncVar(hook = nameof(OnExpChanged))]
    public int exp;

    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };

    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;
    public List<Player> players = new List<Player>();
    //public LevelUp uiLevelUp;
    public Result uiResult;
    public GameObject enemyCleaner;

    public Weapon weapon;   // 추가
    public Gear gear;

    
    public void GameStart(int id)
    {
        //playerId = id;
        health = maxHealth;
        player.gameObject.SetActive(true);

        player.animControllerIndex = id;
        //player.anim.runtimeAnimatorController = player.animCon[GameManager.instance.playerId]; // ?

        enemyCleaner.SetActive(false);

        //uiLevelUp.select(0);
        Resume();
    }

   
    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();
    }

    
    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    IEnumerator GameVictoryRoutine()
    {
        isLive = false;
        enemyCleaner.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        Stop();
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);      // 추후 확인 요
    }


    void Awake()
    {
        instance = this;
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
        //if (isServer)
            GameManager.instance.player.statPoints++; // 레벨 증가만큼 스탯 포인트 지급
    }

    void OnKillChanged(int oldKill, int newKill) { }
    void OnExpChanged(int oldExp, int newExp) { }

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