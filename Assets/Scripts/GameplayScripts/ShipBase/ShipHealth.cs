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
    protected float powerRegen;

    protected float evasion;

    //Current
    public int Health { get; protected set; }
    public int Shield { get; protected set; }
    public float Power { get; protected set; }

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
        int originalDamage = damage;

        //Evasion
        if (Random.value < evasion)
        {
            // Report evasion as damage mitigation to all modules
            ReportDamageMitigated(originalDamage);
            return;
        }

        //Shield damage
        if (Shield >= damage)
        {
            Shield -= damage;
            // Report shield mitigation to modules
            ReportDamageMitigated(originalDamage);
            return;
        }
        else
        {
            // Report shield portion to modules
            ReportDamageMitigated(Shield);
            damage -= Shield;
            Shield = 0;
        }

        //Hull damage
        int armorMitigation = Mathf.RoundToInt(Mathf.Max(armor * armorReduction, 0));
        damage -= armorMitigation;
        
        // Report armor mitigation to modules
        ReportDamageMitigated(armorMitigation);
        
        Health -= Mathf.Max(damage, 1);

        if (Health <= 0)
            OutOfHealth();
    }

    private void ReportDamageMitigated(int mitigatedAmount)
    {
        // Report to all modules (they all contribute to defense)
        if (CombatEventRouter.Instance != null && ModuleManager.Instance != null)
        {
            foreach (var (genome, tracker) in ModuleManager.Instance.GetAllModules())
            {
                if (genome != null)
                {
                    CombatEventRouter.Instance.ReportModuleDamageMitigated(genome.id, mitigatedAmount);
                }
            }
        }
    }
    public void RegainHealth(int regainAmount)
    {
        Health += regainAmount;
        if (Health > HullHP)
            Health = HullHP;
    }
    public bool ConsumePower(float amountConsumed)
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
