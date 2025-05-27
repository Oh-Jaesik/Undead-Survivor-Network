//using Mirror;
//using UnityEngine;

//public class GameManager : NetworkBehaviour
//{
//    public static GameManager instance;

//    [SyncVar]
//    public float gameTime;
//    public float maxGameTime = 2 * 10f;

//    public PoolManager pool;
//    public Player player;

//    void Awake()
//    {
//        instance = this;
//    }

//    [Server]
//    void Update()
//    {
//        gameTime += Time.deltaTime;

//        if (gameTime > maxGameTime)
//        {
//            gameTime = maxGameTime;
//        }
//    }
//}

using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PoolManager pool;
    public float gameTime;

    public List<Player> players = new List<Player>(); // 모든 플레이어 목록

    void Awake()
    {
        instance = this;
    }

    public void RegisterPlayer(Player player)
    {
        if (!players.Contains(player))
            players.Add(player);
    }

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
