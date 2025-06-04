using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
public class StyleEvaluator : MonoBehaviour
{
    public PlayerTracker moveTracker;
    public DistanceTracker distTracker;
    public AttackTracker attackTracker;
    public TimeTracker timeTracker;
    public Text styleText;
    public RectTransform rect;


    //public void Start()
    //{
    //    styleText = GetComponent<Text>();
    //    rect = GetComponent<RectTransform>();
    //    Transform player = rect.parent.parent;
        
    //    moveTracker = player.GetComponent<PlayerTracker>();
    //    distTracker = player.GetComponent<DistanceTracker>();
    //    attackTracker = player.GetComponent<AttackTracker>();
    //    timeTracker = player.GetComponent<TimeTracker>();
    //}

    IEnumerator Start()
    {
        styleText = GetComponent<Text>();
        rect = GetComponent<RectTransform>();

        if (rect == null || rect.parent == null || rect.parent.parent == null)
        {
            Debug.LogWarning("RectTransform 계층이 유효하지 않음");
            yield break;
        }

        Transform player = rect.parent.parent;

        if (player == null)
        {
            Debug.LogWarning("Player transform이 null");
            yield break;
        }

        moveTracker = player.GetComponent<PlayerTracker>();
        distTracker = player.GetComponent<DistanceTracker>();
        attackTracker = player.GetComponent<AttackTracker>();
        timeTracker = player.GetComponent<TimeTracker>();

        yield return new WaitForSeconds(60f);
        rect.localScale = Vector3.one;

        float move = moveTracker.totalMovement;
        float avgDist = distTracker.avgDistance;
        int hitCount = attackTracker.monsterHitCount;
        float playTime = timeTracker.playTime;
        Debug.Log($"보내는 값: dist={avgDist}, hit={hitCount}, move={move}, time={playTime}");

        StartCoroutine(SendToModel(move, avgDist, hitCount, playTime));
    }

    IEnumerator SendToModel(float move, float dist, int hit, float time)
    {
        WWWForm form = new WWWForm();
        form.AddField("avg_distance", dist.ToString());
        form.AddField("hit_count", hit.ToString());
        form.AddField("total_movement", move.ToString());
        form.AddField("play_time", time.ToString());

        UnityWebRequest req = UnityWebRequest.Post("http://127.0.0.1:5000/predict", form); // 주소는 파이참에서 predict_server.py 실행시키고 뜨는 주소로.
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            styleText.text = req.downloadHandler.text;
        }
        else
        {
            styleText.text = "성향 분석 실패 : " + req.error;
        }
    }
}
