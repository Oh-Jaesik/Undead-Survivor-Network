using UnityEngine;

public class DistanceTracker : MonoBehaviour
{
    public float distanceSum = 0f;
    public int sampleCount = 0;
    public float avgDistance => sampleCount > 0 ? distanceSum / sampleCount : 0f;

    private void Update()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Enemy"); // 여기선 Enemy하고 일단 몬스터 태그 따라감
        if (monsters.Length == 0 || Time.timeScale == 0)
            return;

        float closestDistance = float.MaxValue;

        foreach (GameObject monster in monsters)
        {
            float dist = Vector3.Distance(transform.position, monster.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
            }
        }

        distanceSum += closestDistance;
        sampleCount++;
    }
}
