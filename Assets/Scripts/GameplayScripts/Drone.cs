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


    float turn_Rate = 4;
    float minimum_Distance_To_Target = 1.5f;

    //Temp
    Transform shipTransform;

    private void Start()
    {
        rigidbodyThis = GetComponent<Rigidbody2D>();

        //Temp
        shipTransform = GameObject.Find("Ship").transform;
    }
    private void FixedUpdate()
    {
        //Temp
        targetPosition = shipTransform.position - shipTransform.up * 0.5f;


        Move_To_Target(targetPosition);
    }
    private void Move_To_Target(Vector2 target_Position)
    {
        Vector2 targetDirection = (target_Position - (Vector2)transform.position).normalized;
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
        float newAngle = Mathf.LerpAngle(rigidbodyThis.rotation, targetAngle, turn_Rate * Time.fixedDeltaTime);
        rigidbodyThis.MoveRotation(newAngle);

        if (Vector2.Distance(target_Position, rigidbodyThis.position) <= minimum_Distance_To_Target)
            return;

        rigidbodyThis.MovePosition(rigidbodyThis.position + (Vector2)transform.up * speed * Time.fixedDeltaTime);
    }
    public void SetStats(float speed, int durability, float aggression)
    {
        this.speed = speed;
        this.durability = durability;
        this.aggression = aggression;
    }
    public void TakeDamage(int damage)
    {
        damage_Taken += damage;

        if (damage_Taken >= durability)
            Destroy(gameObject);
    }
}
