using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class EnemyMovement : ShipMovement
{
    //Stats
    [SerializeField] float enemyBaseSpeed;
    [SerializeField] float enemyTurnRate;
    [SerializeField] float enemyMass;
    [SerializeField] float enemyInertia;

    [SerializeField] float enemyAngularDamping = 0.01f;


    public Vector2? destination;

    Camera mainCamera;
    protected override void OnStart()
    {
        base.OnStart();

        base.speed = enemyBaseSpeed;
        base.turnRate = enemyTurnRate;
        base.mass = enemyMass;
        base.inertia = enemyInertia;
        base.angularDamping = enemyAngularDamping;

        rigidbodyThis.mass = enemyMass;
        rigidbodyThis.inertia = enemyInertia;

        mainCamera = Camera.main;
    }

    void FixedUpdate()
    {
        Vector2 position = transform.position;

        //Move towards destination
        if (destination.HasValue)
        {
            Vector2 target = destination.Value;



            Vector2 targetDirection = (target - (Vector2)position).normalized;
            float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
            float angleDifference = Mathf.DeltaAngle(rigidbodyThis.rotation, targetAngle);
            rigidbodyThis.AddTorque(angleDifference * turnRate, ForceMode2D.Force);

            rigidbodyThis.linearVelocity = Vector2.zero;
            rigidbodyThis.MovePosition(rigidbodyThis.position + (Vector2)transform.up * Speed * Time.fixedDeltaTime);

        }
        //Teleport enemy back onto screen
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(position);
        if (viewportPosition.x < -0.04f || viewportPosition.x > 1.04f ||
            viewportPosition.y < -0.06f || viewportPosition.y > 1.06f)
        {
            if (viewportPosition.x < -0.04f)
                viewportPosition.x = 1.035f;
            if (viewportPosition.x > 1.04f)
                viewportPosition.x = -0.035f;
            if (viewportPosition.y < -0.06f)
                viewportPosition.y = 1.05f;
            if (viewportPosition.y > 1.06f)
                viewportPosition.y = -0.05f;
            Vector2 newPos = mainCamera.ViewportToWorldPoint(viewportPosition);
            transform.SetPositionAndRotation(newPos, Quaternion.identity);

            //Vector2 direction = (-position).normalized;
            //rigidbodyThis.MovePosition(rigidbodyThis.position + direction * 20 * Time.fixedDeltaTime);
        }

        // Auto-stabilization
        rigidbodyThis.AddTorque(-rigidbodyThis.angularVelocity * angularDamping, ForceMode2D.Force);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Help separate stuck enemies from each other
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 direction = (transform.position - collision.transform.position).normalized;
            rigidbodyThis.MovePosition(rigidbodyThis.position + direction * 2f * Time.fixedDeltaTime);
        }
    }
}
