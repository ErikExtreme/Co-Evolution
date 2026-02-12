using UnityEngine;
using System.Collections.Generic;

public class PlayerBehaviorTracker : MonoBehaviour
{
    public static PlayerBehaviorTracker Instance;

    [Header("Movement")]
    public float avgSpeed;
    public float avgTurnRate;
    public float movementEntropy;

    [Header("Combat Positioning")]
    public float avgEngagementDistance;
    public float avgAngleToEnemy;

    [Header("Threat Priority")]
    public Dictionary<string, int> enemyTypeFocus = new();

    float smoothing = 0.1f;
    Vector3 lastPosition;

    void Awake()
    {
        Instance = this;
        lastPosition = transform.position;
    }

    void Update()
    {
        TrackMovement();
        TrackEngagementDistance();
        TrackAngleToEnemy();
    }

    void TrackMovement()
    {
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        avgSpeed = Mathf.Lerp(avgSpeed, speed, smoothing);

        float turn = Vector3.Angle(transform.forward, (transform.position - lastPosition).normalized);
        avgTurnRate = Mathf.Lerp(avgTurnRate, turn, smoothing);

        movementEntropy = Mathf.Lerp(movementEntropy, turn * speed, smoothing);

        lastPosition = transform.position;
    }

    void TrackEngagementDistance()
    {
        Transform enemy = EnemyManager.Instance.GetClosestEnemy(transform.position);
        if (enemy == null) return;

        float d = Vector3.Distance(transform.position, enemy.position);
        avgEngagementDistance = Mathf.Lerp(avgEngagementDistance, d, smoothing);
    }

    void TrackAngleToEnemy()
    {
        Transform enemy = EnemyManager.Instance.GetClosestEnemy(transform.position);
        if (enemy == null) return;

        Vector3 dir = (enemy.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dir);
        avgAngleToEnemy = Mathf.Lerp(avgAngleToEnemy, angle, smoothing);
    }

    public void RegisterEnemyFocus(string enemyType)
    {
        if (!enemyTypeFocus.ContainsKey(enemyType))
            enemyTypeFocus[enemyType] = 0;

        enemyTypeFocus[enemyType]++;
    }
}
