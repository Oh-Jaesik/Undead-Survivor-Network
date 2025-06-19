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
    public Text styleText;         // �� ����� ��� Text
    public RectTransform rect;     // UI RectTransform

    [SyncVar(hook = nameof(OnPlayStyleChanged))]  // ���� �� Ŭ���̾�Ʈ���� UI ������Ʈ
    public string playStyle;

    // �÷��̾� ������
    private float avg_distance;
    private float hit_count;
    private float total_movement;
    private float play_time;

    private Transform player;
    private TreeNode rootNode;
    private bool hasStarted = false;

    void Start()
    {
        // ������Ʈ Ȯ��
        if (styleText == null) styleText = GetComponent<Text>();
        if (rect == null) rect = GetComponent<RectTransform>();

        // NetworkIdentity �������� �÷��̾� ã��
        player = transform.root;  // ���� �ٱ��� ��Ʈ�� player

        rootNode = LoadTreeFromJson();
    }

    private void Update()
    {
        if (!isServer || hasStarted) return;

        if (GameManager.instance.gameTime >= 40f)
        {
            hasStarted = true;
            StartCoroutine(EvaluateAfterDelay(rootNode));
        }
    }


    TreeNode LoadTreeFromJson()
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>("decision_tree_rules");
        if (jsonTextAsset != null)
        {
            Debug.Log("�ҷ��� JSON: " + jsonTextAsset.text);

            // Newtonsoft.Json�� ����� �Ľ�
            TreeNode node = JsonConvert.DeserializeObject<TreeNode>(jsonTextAsset.text);

            Debug.Log("Ʈ�� ��Ʈ feature: " + node.feature);
            Debug.Log("Ʈ�� ��Ʈ threshold: " + node.threshold);
            Debug.Log("Ʈ�� ���� �ڽ�: " + (node.left != null ? node.left.feature : "null"));
            Debug.Log("Ʈ�� ������ �ڽ�: " + (node.right != null ? node.right.feature : "null"));
            return node;
        }
        else
        {
            Debug.LogError("JSON ������ ã�� �� �����ϴ�!");
            return null;
        }
    }

    IEnumerator EvaluateAfterDelay(TreeNode node)
    {
        //yield return new WaitForSeconds(delay);
        yield return null;

        // �÷��̾� ������ �б�
        avg_distance = player.GetComponent<DistanceTracker>().avgDistance;
        hit_count = player.GetComponent<AttackTracker>().monsterHitCount;
        total_movement = player.GetComponent<PlayerTracker>().totalMovement;
        //play_time = player.GetComponent<TimeTracker>().playTime;
        play_time = GameManager.instance.gameTime;

        Debug.Log($"�������� �� ����: dist={avg_distance}, hit={hit_count}, total={total_movement}, time={play_time}");

        // ���� �÷��� ��Ÿ�� ��
        playStyle = EvaluatePlayStyle(node, avg_distance, hit_count, total_movement, play_time);
        Debug.Log("�������� ���� �� ���: " + playStyle);

    }

    string EvaluatePlayStyle(TreeNode node, float avg_distance, float hit_count, float total_movement, float play_time)
    {
        if (node == null)
            return "Unknown";

        if (!string.IsNullOrEmpty(node.@class))
            return node.@class;  // �з� ���!

        float featureValue = 0f;
        switch (node.feature.Trim().ToLower())
        {
            case "avg_distance": featureValue = avg_distance; break;
            case "hit_count": featureValue = hit_count; break;
            case "total_movement": featureValue = total_movement; break;
            case "play_time": featureValue = play_time; break;
            default:
                Debug.LogError("����ġ ���� feature: " + node.feature);
                return "Unknown";
        }

        if (featureValue <= node.threshold)
            return node.left != null ? EvaluatePlayStyle(node.left, avg_distance, hit_count, total_movement, play_time)
                                     : (node.left?.@class ?? "Unknown");
        else
            return node.right != null ? EvaluatePlayStyle(node.right, avg_distance, hit_count, total_movement, play_time)
                                      : (node.right?.@class ?? "Unknown");
    }


    // Ŭ���̾�Ʈ: SyncVar ���� �ٲ� �� ���� (UI�� ����!)
    void OnPlayStyleChanged(string oldStyle, string newStyle)
    {
        styleText.text = newStyle;
        rect.localScale = Vector3.one;
        Debug.Log("Ŭ���̾�Ʈ���� �÷��� ��Ÿ�� ǥ��: " + newStyle);
    }
}
