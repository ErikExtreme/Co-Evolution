using UnityEngine;

public class EnemyTargeting : MonoBehaviour
{
    private EnemyMovement movementScript;
    private Camera camera;

    private Transform target;

    [SerializeField] private float targetDistanceFromTarget;
    [SerializeField] private float stopDistance = 0.1f;
    void Start()
    {
        movementScript = GetComponent<EnemyMovement>();
        camera = Camera.main;

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

        //Clamp destination to on screen
        Vector3 viewportPosition = camera.WorldToViewportPoint(destination);
        viewportPosition.x = Mathf.Clamp01(viewportPosition.x);
        viewportPosition.y = Mathf.Clamp01(viewportPosition.y);
        destination = camera.ViewportToWorldPoint(viewportPosition);

        if (Vector2.Distance(transform.position, destination) < stopDistance)
        {
            movementScript.destination = null;
            return;
        }

        movementScript.destination = destination;
    }
}
