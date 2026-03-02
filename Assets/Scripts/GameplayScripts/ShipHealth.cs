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
    protected int health;
    protected int shield;
    protected int power;

    protected float regenTimer;

    //Modifiers
    [System.NonSerialized] public float armorReduction;

    void Start()
    {
        OnStart();
    }
    protected virtual void OnStart()
    {
        health = hullHP;
        shield = shieldCapacity;
        power = powerCapacity;

        regenTimer = 1;

        armorReduction = 0;
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
        else
        {
            damage -= shield;
            shield = 0;
        }

        //Hull damage
        damage -= Mathf.RoundToInt(Mathf.Max(armor * armorReduction, 0));
        health -= Mathf.Max(damage, 1);

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
