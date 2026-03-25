using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponGenome weapon_Genome;
    public WeaponStatsTracker tracker;

    public ShipHealth shipHealthScript;

    [SerializeField] GameObject projectilePrefab;
    protected string opponentTag;

    protected float chargeUpTimer;

    float currentHeat;
    int maxHeat = 100;
    bool isOverHeated;

    int bullets_Left_In_Burst;
    float cooldown_Timer;

    protected float shoot_Timer;


    protected float projectileSpeedAdjusted;
    protected float fireRateAdjusted;
    protected float chargeUpTimeAdjusted;

    private Transform target;

    public WeaponBoost activeBoost;
    //Values for boosts (standalone, all additive)
    int damageBoostAdditive = 10;
    float burstRateBoost = 0.5f;
    float spreadAngleBoost = 10;
    int heatBoost = 5;
    //Values for boosts (Buff + debuff, multiplicative)
    float damageBoostMultiplier = 2;
    float heatPenalty = 2;
    float aoeRangeBoost = 1.5f;
    float burstRatePenalty = 0.66f;

    void Start()
    {
        OnStart();
    }
    protected virtual void OnStart()
    {
        chargeUpTimeAdjusted = weapon_Genome.chargeUpTime;
        fireRateAdjusted = weapon_Genome.fireRate;
        projectileSpeedAdjusted= weapon_Genome.projectileSpeed;

        chargeUpTimer = chargeUpTimeAdjusted;
        shoot_Timer = 1 / Mathf.Max(fireRateAdjusted, 0.001f);
        bullets_Left_In_Burst = 0;

        opponentTag = "Enemy";
    }

    public void NewTarget(Transform target)
    {
        this.target = target;
    }

    public abstract Transform FindNewTarget();

    void Update()
    {
        if (!isOverHeated)
        {
            HandleShooting();
        }

        currentHeat -= Time.deltaTime * weapon_Genome.heatDissipation;//Only dissipate heat while overheated?, slower dissipation when not overheated?
        currentHeat = Mathf.Max(currentHeat, 0);

        if (currentHeat >= maxHeat)
        {
            isOverHeated = true;
            chargeUpTimer = chargeUpTimeAdjusted;
        }
        else if (currentHeat <= 0)
            isOverHeated = false;
    }

    private void HandleShooting()
    {
        chargeUpTimer -= Time.deltaTime;
        if (chargeUpTimer > 0)//Need to charge up again if weapon hasnt shot for X seconds?
        {
            return;
        }

        if (bullets_Left_In_Burst <= 0)
        {
            HandleBurstRefill();
            return;
        }

        if (target == null || !target.gameObject.activeSelf)
        {
            target = FindNewTarget();
            if (target == null) return;
        }

        shoot_Timer -= Time.deltaTime;
        if (shoot_Timer <= 0 && bullets_Left_In_Burst > 0)
            FireBullet();
    }
    private void HandleBurstRefill()
    {
        cooldown_Timer -= Time.deltaTime;
        if (cooldown_Timer > 0)
            return;

        if (!shipHealthScript.ConsumePower(weapon_Genome.powerCost))
            return;

        cooldown_Timer = weapon_Genome.cooldownTime;
        if (activeBoost == WeaponBoost.BurstRate)
            cooldown_Timer -= burstRateBoost;
        if (activeBoost == WeaponBoost.BiggerAOELessBurstRate)
            cooldown_Timer *= burstRatePenalty;

        bullets_Left_In_Burst = weapon_Genome.burstSize;
    }
    private void FireBullet()
    {
        Vector2 targetDirection = ((Vector2)target.position - (Vector2)transform.position).normalized;
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;

        Quaternion rotation;
        if (Random.value >= weapon_Genome.accuracy)
        {
            float possibleAngle = weapon_Genome.spreadAngle;
            if (activeBoost == WeaponBoost.Accuracy)
                possibleAngle = Mathf.Max(possibleAngle - spreadAngleBoost, 0);
            float spreadAngle = Random.Range(-possibleAngle, possibleAngle);
            rotation = Quaternion.Euler(0, 0, targetAngle + spreadAngle);
        }
        else
            rotation = Quaternion.Euler(0, 0, targetAngle);

        GameObject projectile_Instance = Instantiate(projectilePrefab, transform.position, rotation);
        int damage = weapon_Genome.baseDamage;
        float aoeRadius = weapon_Genome.aoeRadius;
        if (activeBoost == WeaponBoost.Damage)
            damage += Mathf.RoundToInt(damageBoostAdditive);
        if (activeBoost == WeaponBoost.MoreDamageMoreHeat)
            damage = Mathf.RoundToInt(damage * damageBoostMultiplier);
        if (activeBoost == WeaponBoost.BiggerAOELessBurstRate)
            aoeRadius *= aoeRangeBoost;

        projectile_Instance.GetComponent<Projectile>().SetInitialValues(damage, projectileSpeedAdjusted, weapon_Genome.range, aoeRadius, weapon_Genome.statusEffectType, weapon_Genome.statusEffectStrength, opponentTag);


        bullets_Left_In_Burst--;
        shoot_Timer = 1 / Mathf.Max(fireRateAdjusted, 0.001f);
        int heatIncrease = weapon_Genome.heatPerShot;
        if (activeBoost == WeaponBoost.LowerHeatGeneration)
            heatIncrease -= heatBoost;
        if (activeBoost == WeaponBoost.MoreDamageMoreHeat)
            heatIncrease = Mathf.RoundToInt(heatIncrease * heatPenalty);
        currentHeat += heatIncrease;

        //Rotates weapon to point in shooting direction
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }
}

public enum WeaponBoost
{
    None,
    Damage,
    BurstRate,
    Accuracy,
    LowerHeatGeneration,
    MoreDamageMoreHeat,
    BiggerAOELessBurstRate
}
