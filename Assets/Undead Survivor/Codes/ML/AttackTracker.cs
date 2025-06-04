using UnityEngine;

public class AttackTracker : MonoBehaviour
{
    public int monsterHitCount = 0;

    public void RegisterHit()
    {
        monsterHitCount++;
    }
}
