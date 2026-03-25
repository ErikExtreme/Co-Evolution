using UnityEngine;
using UnityEngine.InputSystem;

public class Drone : MonoBehaviour
{
    //Stats
    float speed;
    int durability;
    float aggression;


    //Variables
    private Rigidbody2D rigidbodyThis;
    Vector2 targetPosition;
    float damage_Taken;

    float minimum_Distance_To_Target = 1f;

    Transform shipTransform;
    Vector2 randomPositionOffset;

    ShipDroneManager shipDroneManager;

    private void Start()
    {
        rigidbodyThis = GetComponent<Rigidbody2D>();

        shipTransform = GameObject.Find("Ship").transform;
        randomPositionOffset = Random.insideUnitCircle;
    }
    private void FixedUpdate()
    {
        Transform closestEnemy = EnemyManager.Instance.GetClosestEnemy(transform.position);
        if (closestEnemy != null)
        {
            Vector3 playerEnemyMidPoint = Vector3.Lerp(shipTransform.position, closestEnemy.position + closestEnemy.up * 0.4f, aggression);

            //if (Vector2.Distance(targetPosition, playerEnemyMidPoint) >= 1f)
                targetPosition = playerEnemyMidPoint;
        }

        Move_To_Target(targetPosition+randomPositionOffset);
        rigidbodyThis.linearVelocity = Vector2.zero;
    }
    private void Move_To_Target(Vector2 target_Position)
    {
        Vector2 targetDirection = (target_Position - (Vector2)transform.position).normalized;

        if (Vector2.Distance(target_Position, rigidbodyThis.position) <= minimum_Distance_To_Target)
            return;

        rigidbodyThis.MovePosition(rigidbodyThis.position + targetDirection * speed * Time.fixedDeltaTime);
    }
    public void SetStats(float speed, int durability, float aggression, ShipDroneManager shipDroneManager)
    {
        this.speed = speed;
        this.durability = durability;
        this.aggression = aggression * 0.9f + 0.1f;//remaps 0-1 range to 0.1-1 range

        this.shipDroneManager = shipDroneManager;
    }
    public void TakeDamage(int damage)
    {
        damage_Taken += damage;

        if (damage_Taken >= durability)
            shipDroneManager.DestroyDrone(this);
    }
}
