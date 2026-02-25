using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ShipHealth : MonoBehaviour
{
    //Stats
    protected int hullHP;
    protected int armor;
    protected int shieldCapacity;
    protected int shieldRegen;
    protected int powerCapacity;
    protected int powerRegen;

    protected float evasion;

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
    public void SetStats(ShipCoreStats shipCoreStats)
    {
        //Stats
        hullHP = shipCoreStats.hullHP;
        armor = shipCoreStats.armor;
        shieldCapacity = shipCoreStats.shieldCapacity;
        shieldRegen = shipCoreStats.shieldRegen;
        powerCapacity = shipCoreStats.powerCapacity;
        powerRegen = shipCoreStats.powerRegen;

        evasion = shipCoreStats.evasion;

        //Current, shouldn't necessarly be set here, depends on design
        health = hullHP;
        shield = shieldCapacity;
        power = powerCapacity;
    }

    public void TakeDamage(int damage)
    {
        //Evasion
        if (Random.value < evasion)
            return;

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
            OutOfHealth();
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
    protected virtual void OutOfHealth()
    {
        Debug.Log("RAN OUT OF HEALTH");//lose in some way
    }
}
