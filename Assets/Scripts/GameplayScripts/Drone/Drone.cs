using UnityEngine;
using UnityEngine.InputSystem;

public class Drone : ShipHealth
{
    //Stats
    float speed;
    float aggression;

    //Variables
    private Rigidbody2D rigidbodyThis;
    Vector2 targetPosition;

    float minimum_Distance_To_Target = 1f;

    Transform shipTransform;
    Vector2 randomPositionOffset;

    ShipDroneManager shipDroneManager;

    ModuleStatsTracker moduleStatsTracker;

    private void Start()
    {
        rigidbodyThis = GetComponent<Rigidbody2D>();

        shipTransform = GameObject.Find("Ship").transform;
        randomPositionOffset = Random.insideUnitCircle;

        transform.GetChild(0).GetComponent<Weapon>().AddTracker(moduleStatsTracker);
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

        Move_To_Target(targetPosition + randomPositionOffset);
        rigidbodyThis.linearVelocity = Vector2.zero;
    }
    private void Move_To_Target(Vector2 target_Position)
    {
        Vector2 targetDirection = (target_Position - (Vector2)transform.position).normalized;

        if (Vector2.Distance(target_Position, rigidbodyThis.position) <= minimum_Distance_To_Target)
            return;

        rigidbodyThis.MovePosition(rigidbodyThis.position + targetDirection * speed * Time.fixedDeltaTime);
    }
    public void SetStats(float speed, int durability, float aggression, ShipDroneManager shipDroneManager, ModuleStatsTracker moduleStatsTracker)
    {
        this.speed = speed;
        this.aggression = aggression * 0.9f + 0.1f;//remaps 0-1 range to 0.1-1 range

        this.shipDroneManager = shipDroneManager;


        HullHP = durability * 3;
        armor = 0;
        ShieldCapacity = 0;
        shieldRegen = 0;
        PowerCapacity = 10;
        powerRegen = 10;
        evasion = 0;

        this.moduleStatsTracker = moduleStatsTracker;
    }
    public override bool TakeDamage(float damage)
    {
        moduleStatsTracker.RegisterDroneDamageTaken(Mathf.Max(damage, 1));

        Health -= Mathf.Max(damage, 1);

        if (Health <= 0)
        {
            OutOfHealth();
            return true;
        }
        return false;
    }
    protected override void OutOfHealth()
    {
        shipDroneManager.DestroyDrone(this);
    }
}
