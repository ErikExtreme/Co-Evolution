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
    public float Health { get; protected set; }
    public float Shield { get; protected set; }
    public float Power { get; protected set; }

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

        armorReduction = 1;
    }

    void Update()
    {
        if (Shield < ShieldCapacity)
            Shield += shieldRegen * Time.deltaTime;

        if (Power < PowerCapacity)
            Power += powerRegen * Time.deltaTime;
    }

    public virtual bool TakeDamage(float damage)
    {
        float originalDamage = damage;

        //Evasion
        if (Random.value < evasion)
        {
            // Report evasion as damage mitigation to all modules
            ReportDamageMitigated(originalDamage);
            return false;
        }

        //Shield damage
        if (Shield >= damage)
        {
            Shield -= damage;
            // Report shield mitigation to modules
            ReportDamageMitigated(originalDamage);
            return false;
        }
        else
        {
            // Report shield portion to modules
            ReportDamageMitigated(Shield);
            damage -= Shield;
            Shield = 0;
        }

        //Hull damage
        float armorMitigation = Mathf.Max(armor * armorReduction, 0);
        damage -= armorMitigation;
        
        // Report armor mitigation to modules
        ReportDamageMitigated(armorMitigation);
        
        Health -= Mathf.Max(damage, 1);

        if (Health <= 0)
        {
            OutOfHealth();
            return true;
        }
        return false;
    }

    private void ReportDamageMitigated(float mitigatedAmount)
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
        if (Power <= 0)
            return false;

        Power -= amountConsumed;

        return true;
    }
    protected virtual void OutOfHealth()
    {
        Debug.LogWarning("Ran out of health, Death not implemented");
    }
}
