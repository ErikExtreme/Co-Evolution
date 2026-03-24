using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [SerializeField] WaveManager waveManager;

    void Awake()
    {
        Instance = this;
    }

    public Transform GetClosestEnemy(Vector2 pos)
    {
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (var enemy in waveManager.enemiesLeftInWave)
        {
            float distanceToEnemy = Vector2.Distance(enemy.transform.position, pos);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }
        return closestEnemy;
    }
}
