using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ShipHealth : MonoBehaviour
{
    //Stats
    int hullHP=1;
    int armor = 1;
    int shieldCapacity = 1;
    int shieldRegen = 1;
    int powerCapacity = 1;
    int powerRegen = 1;

    //Current
    private int health;
    private int shield;
    private int power;

    float regenTimer;

    void Start()
    {
        health = hullHP;
        shield = shieldCapacity;
        power = powerCapacity;

        regenTimer = 1;
    }

    void Update()
    {
        regenTimer -= Time.deltaTime;
        if (regenTimer <= 0)
        {
            regenTimer = 1;

            if (shield < shieldCapacity)
                shield += shieldRegen;

            if (power < powerCapacity)
                power += powerRegen;
        }
    }

    public void TakeDamage(int damage)
    {
        //Shield damage
        if (shield >= damage)
        {
            shield -= damage;
            return;
        }

        damage -= shield;
        damage -= armor;
        shield = 0;

        //Hull damage
        if (damage > 0)
            health -= damage;
        else
            health -= 1;

        if (health <= 0)
            Debug.Log("RAN OUT OF HEALTH");//lose in some way
    }
    public void RegainHealth(int regainAmount)
    {
        health += regainAmount;
        if (health > hullHP)
            health = hullHP;
    }
    public bool ConsumePower(int amountConsumed)
    {
        if (power >= amountConsumed)
        {
            power -= amountConsumed;
            return true;
        }
        else
            return false;
    }
}
