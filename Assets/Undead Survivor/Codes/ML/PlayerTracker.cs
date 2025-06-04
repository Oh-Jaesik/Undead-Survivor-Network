using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    public float totalMovement = 0f;
    private Vector3 lastPos;

    void Start()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif

        lastPos = transform.position;
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, lastPos);
        totalMovement += dist;
        lastPos = transform.position;
    }
}
