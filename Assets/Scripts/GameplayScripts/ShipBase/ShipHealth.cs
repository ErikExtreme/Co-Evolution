using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ShipHealth : MonoBehaviour
{
    //Stats
    public int HullHP { get; protected set; }
    protected int armor;
    public int ShieldCapacity { get; protected set; }
    protected int shieldRegen;
    public int PowerCapacity { get; protected set; }
    protected int powerRegen;

    protected float evasion;

    //Current
    public int Health { get; protected set; }
    public int Shield { get; protected set; }
    public int Power { get; protected set; }

    protected float regenTimer;

    //Modifiers
    [System.NonSerialized] public float armorReduction;

    void Start()
    {
        OnStart();
    }
    protected virtual void OnStart()
    {
        Health = HullHP;
        Shield = ShieldCapacity;
        Power = PowerCapacity;

        regenTimer = 1;

        armorReduction = 1;
    }

    void Update()
    {
        regenTimer -= Time.deltaTime;
        if (regenTimer <= 0)
        {
            regenTimer = 1;

            if (Shield < ShieldCapacity)
                Shield += shieldRegen;

            if (Power < PowerCapacity)
                Power += powerRegen;
        }
    }

    public void TakeDamage(int damage)
    {
        //Evasion
        if (Random.value < evasion)
            return;

        //Shield damage
        if (Shield >= damage)
        {
            Shield -= damage;
            return;
        }
        else
        {
            damage -= Shield;
            Shield = 0;
        }

        //Hull damage
        damage -= Mathf.RoundToInt(Mathf.Max(armor * armorReduction, 0));
        Health -= Mathf.Max(damage, 1);

        if (Health <= 0)
            OutOfHealth();
    }
    public void RegainHealth(int regainAmount)
    {
        Health += regainAmount;
        if (Health > HullHP)
            Health = HullHP;
    }
    public bool ConsumePower(int amountConsumed)
    {
        if (Power >= amountConsumed)
        {
            Power -= amountConsumed;
            return true;
        }
        else
            return false;
    }
    protected virtual void OutOfHealth()
    {
        Debug.LogWarning("Ran out of health, Death not implemented");
    }
}
