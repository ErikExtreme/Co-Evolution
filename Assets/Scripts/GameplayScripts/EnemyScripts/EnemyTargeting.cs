using UnityEngine;

public class EnemyTargeting : MonoBehaviour
{
    private EnemyMovement movementScript;

    private Transform target;

    [SerializeField] private float targetDistanceFromTarget;
    [SerializeField] private float stopDistance = 0.1f;
    void Start()
    {
        movementScript = GetComponent<EnemyMovement>();

        target = GameObject.FindGameObjectWithTag("Player").transform;
        foreach (var weapon in GetComponentsInChildren<EnemyWeapon>())
        {
            weapon.NewTarget(target);
        }
    }

    void Update()
    {
        if (target != null)
        {
            SetDestination();
        }
    }

    private void SetDestination()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        Vector2 destination = (Vector2)target.position - direction * targetDistanceFromTarget;

        if (Vector2.Distance(transform.position, destination) < stopDistance)
        {
            movementScript.destination = null;
            return;
        }

        movementScript.destination = destination;
    }
}
