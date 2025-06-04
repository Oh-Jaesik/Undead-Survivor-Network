using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;


[System.Serializable]
public class TreeNode
{
    public string feature;
    public float threshold;
    public string @class;
    public TreeNode left;
    public TreeNode right;
}

public class PlayerStyleText : NetworkBehaviour
{
    public Text styleText;         // 평가 결과를 띄울 Text
    public RectTransform rect;     // UI RectTransform

    [SyncVar(hook = nameof(OnPlayStyleChanged))]  // 변경 시 클라이언트에서 UI 업데이트
    public string playStyle;

    // 플레이어 데이터
    private float avg_distance;
    private float hit_count;
    private float total_movement;
    private float play_time;

    private Transform player;
    private TreeNode rootNode;

    void Start()
    {
        // 컴포넌트 확인
        if (styleText == null) styleText = GetComponent<Text>();
        if (rect == null) rect = GetComponent<RectTransform>();

        // NetworkIdentity 기준으로 플레이어 찾기
        player = transform.root;  // 가장 바깥의 루트가 player

        rootNode = LoadTreeFromJson();

        // 서버에서만 60초 후 평가 시작
        if (isServer)
            StartCoroutine(EvaluateAfterDelay(60f, rootNode));
    }

    TreeNode LoadTreeFromJson()
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>("decision_tree_rules");
        if (jsonTextAsset != null)
        {
            Debug.Log("불러온 JSON: " + jsonTextAsset.text);

            // Newtonsoft.Json을 사용해 파싱
            TreeNode node = JsonConvert.DeserializeObject<TreeNode>(jsonTextAsset.text);

            Debug.Log("트리 루트 feature: " + node.feature);
            Debug.Log("트리 루트 threshold: " + node.threshold);
            Debug.Log("트리 왼쪽 자식: " + (node.left != null ? node.left.feature : "null"));
            Debug.Log("트리 오른쪽 자식: " + (node.right != null ? node.right.feature : "null"));
            return node;
        }
        else
        {
            Debug.LogError("JSON 파일을 찾을 수 없습니다!");
            return null;
        }
    }

    IEnumerator EvaluateAfterDelay(float delay, TreeNode node)
    {
        yield return new WaitForSeconds(delay);

        // 플레이어 데이터 읽기
        avg_distance = player.GetComponent<DistanceTracker>().avgDistance;
        hit_count = player.GetComponent<AttackTracker>().monsterHitCount;
        total_movement = player.GetComponent<PlayerTracker>().totalMovement;
        play_time = player.GetComponent<TimeTracker>().playTime;

        Debug.Log($"서버에서 평가 시작: dist={avg_distance}, hit={hit_count}, total={total_movement}, time={play_time}");

        // 최종 플레이 스타일 평가
        playStyle = EvaluatePlayStyle(node, avg_distance, hit_count, total_movement, play_time);
        Debug.Log("서버에서 최종 평가 결과: " + playStyle);
    }

    string EvaluatePlayStyle(TreeNode node, float avg_distance, float hit_count, float total_movement, float play_time)
    {
        if (node == null)
            return "Unknown";

        if (!string.IsNullOrEmpty(node.@class))
            return node.@class;  // 분류 결과!

        float featureValue = 0f;
        switch (node.feature.Trim().ToLower())
        {
            case "avg_distance": featureValue = avg_distance; break;
            case "hit_count": featureValue = hit_count; break;
            case "total_movement": featureValue = total_movement; break;
            case "play_time": featureValue = play_time; break;
            default:
                Debug.LogError("예상치 못한 feature: " + node.feature);
                return "Unknown";
        }

        if (featureValue <= node.threshold)
            return node.left != null ? EvaluatePlayStyle(node.left, avg_distance, hit_count, total_movement, play_time)
                                     : (node.left?.@class ?? "Unknown");
        else
            return node.right != null ? EvaluatePlayStyle(node.right, avg_distance, hit_count, total_movement, play_time)
                                      : (node.right?.@class ?? "Unknown");
    }


    // 클라이언트: SyncVar 값이 바뀔 때 실행 (UI만 갱신!)
    void OnPlayStyleChanged(string oldStyle, string newStyle)
    {
        styleText.text = newStyle;
        rect.localScale = Vector3.one;
        Debug.Log("클라이언트에서 플레이 스타일 표시: " + newStyle);
    }
}
