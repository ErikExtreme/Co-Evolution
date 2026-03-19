using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyMovement : ShipMovement
{
    //Stats
    [SerializeField] float enemyBaseSpeed;
    [SerializeField] float enemyTurnRate;
    [SerializeField] float enemyMass;
    [SerializeField] float enemyInertia;

    [SerializeField] float enemyAngularDamping = 0.01f;


    public Vector2? destination;

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
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (destination.HasValue)
        {
            Vector2 target = destination.Value;

            Vector2 targetDirection = (target - (Vector2)transform.position).normalized;
            float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
            float angleDifference = Mathf.DeltaAngle(rigidbodyThis.rotation, targetAngle);
            rigidbodyThis.AddTorque(angleDifference * turnRate, ForceMode2D.Force);

            rigidbodyThis.linearVelocity = Vector2.zero;
            rigidbodyThis.MovePosition(rigidbodyThis.position + (Vector2)transform.up * Speed * Time.fixedDeltaTime);
        }

        // Auto-stabilization
        rigidbodyThis.AddTorque(-rigidbodyThis.angularVelocity * angularDamping, ForceMode2D.Force);
    }
}
