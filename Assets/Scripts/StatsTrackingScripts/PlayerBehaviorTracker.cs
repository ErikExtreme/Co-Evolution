using UnityEngine;
using System.Collections.Generic;

public class PlayerBehaviorTracker : MonoBehaviour
{
    public static PlayerBehaviorTracker Instance;

    [Header("Raw Behavior")]
    public float avgSpeed;
    public float avgTurnRate;
    public float movementEntropy;
    public float avgEngagementDistance;
    public float avgAngleToEnemy;

    [Header("Normalized Behavior (Z-Scores)")]
    public float normSpeed;
    public float normTurnRate;
    public float normEntropy;
    public float normEngagementDistance;
    public float normAngleToEnemy;

    // Rolling statistics
    private RollingStat speedStat = new RollingStat();
    private RollingStat turnStat = new RollingStat();
    private RollingStat entropyStat = new RollingStat();
    private RollingStat distanceStat = new RollingStat();
    private RollingStat angleStat = new RollingStat();

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
        UpdateNormalizedValues();
    }

    void TrackMovement()
    {
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        avgSpeed = Mathf.Lerp(avgSpeed, speed, smoothing);
        speedStat.AddSample(avgSpeed);

        float turn = Vector3.Angle(transform.forward, (transform.position - lastPosition).normalized);
        avgTurnRate = Mathf.Lerp(avgTurnRate, turn, smoothing);
        turnStat.AddSample(avgTurnRate);

        movementEntropy = Mathf.Lerp(movementEntropy, turn * speed, smoothing);
        entropyStat.AddSample(movementEntropy);

        lastPosition = transform.position;
    }

    void TrackEngagementDistance()
    {
        Transform enemy = EnemyManager.Instance.GetClosestEnemy(transform.position);
        if (enemy == null) return;

        float d = Vector3.Distance(transform.position, enemy.position);
        avgEngagementDistance = Mathf.Lerp(avgEngagementDistance, d, smoothing);
        distanceStat.AddSample(avgEngagementDistance);
    }

    void TrackAngleToEnemy()
    {
        Transform enemy = EnemyManager.Instance.GetClosestEnemy(transform.position);
        if (enemy == null) return;

        Vector3 dir = (enemy.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dir);
        avgAngleToEnemy = Mathf.Lerp(avgAngleToEnemy, angle, smoothing);
        angleStat.AddSample(avgAngleToEnemy);
    }

    void UpdateNormalizedValues()
    {
        normSpeed = speedStat.ZScore(avgSpeed);
        normTurnRate = turnStat.ZScore(avgTurnRate);
        normEntropy = entropyStat.ZScore(movementEntropy);
        normEngagementDistance = distanceStat.ZScore(avgEngagementDistance);
        normAngleToEnemy = angleStat.ZScore(avgAngleToEnemy);
    }
}

public class RollingStat
{
    private int n = 0;
    private float mean = 0f;
    private float m2 = 0f; // sum of squares of differences from the mean

    public void AddSample(float x)
    {
        n++;
        float delta = x - mean;
        mean += delta / n;
        float delta2 = x - mean;
        m2 += delta * delta2;
    }

    public float Mean => mean;

    public float Variance => (n > 1) ? m2 / (n - 1) : 0f;

    public float StdDev => Mathf.Sqrt(Variance);

    public float ZScore(float x)
    {
        float sd = StdDev;
        if (sd < 0.0001f) return 0f; // avoid division by zero
        return Mathf.Clamp((x - mean) / sd, -3f, 3f); // clamp extreme outliers
    }
}
