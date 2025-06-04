using UnityEngine;

public class TimeTracker : MonoBehaviour
{
    public float playTime = 0f;

    void Update()
    {
        playTime += Time.deltaTime;
    }
}
