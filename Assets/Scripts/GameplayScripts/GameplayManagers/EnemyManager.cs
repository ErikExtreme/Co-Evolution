using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public Transform GetClosestEnemy(Vector3 pos)
    {
        return transform;
    }
}
