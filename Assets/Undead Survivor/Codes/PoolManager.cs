using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UIElements;

public class PoolManager : NetworkBehaviour
{
    public GameObject[] prefabs;
    List<GameObject>[] pools;

    private void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];

        for (int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
        }
    }

    public GameObject Get(int index, Vector3 position, Quaternion rotation)
    {
        GameObject select = null;

        // 선택한 풀의 비활성화된 게임 오브젝트 접근
        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf)
            {
                // 발견하면 select 변수에 할당
                select = item;

                select.transform.position = position;
                select.transform.rotation = rotation;

                //select.SetActive(true);
                NetworkServer.Spawn(select);

                break;
            }
        }

        if (!select)
        {
            select = Instantiate(prefabs[index], position, rotation);
            //select.SetActive(false);
            NetworkServer.Spawn(select);
        }

        return select;
    }

    [Server]
    public void ReturnToPool(GameObject obj)
    {
        NetworkServer.UnSpawn(obj); // <--- ★★★ 중요: 네트워크에서 오브젝트를 명시적으로 스폰 해제 ★★★
        obj.SetActive(false); // 오브젝트를 비활성화합니다.
    }
}