using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyMovement : MonoBehaviour
{
    //Stats
    [SerializeField] float speed;
    [SerializeField] float turnRate;
    [SerializeField] float mass;
    [SerializeField] float inertia;

    [SerializeField] float angularDamping = 0.05f;

    private Rigidbody2D rigidbodyThis;

    public Vector2? destination;

    void Start()
    {
        rigidbodyThis = GetComponent<Rigidbody2D>();
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
            rigidbodyThis.MovePosition(rigidbodyThis.position + (Vector2)transform.up * speed * Time.fixedDeltaTime);
        }

        // Auto-stabilization
        rigidbodyThis.AddTorque(- rigidbodyThis.angularVelocity * angularDamping, ForceMode2D.Force);
    }
}
