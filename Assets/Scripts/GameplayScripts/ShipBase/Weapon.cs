using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponGenome weapon_Genome;
    public WeaponStatsTracker tracker;

    public ShipHealth shipHealthScript;

    [SerializeField] GameObject projectilePrefab;
    [SerializeField] protected string opponentTag;

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
    protected float powerCostAdjusted;
    protected int damageAdjusted;

    private Transform target;

    public WeaponBoost activeBoost;
    public float boostTier;
    //Values for boosts (standalone, all additive)
    int damageBoostAdditive = 10;
    float burstRateBoost = 0.5f;
    float spreadAngleBoost = 10;
    int heatBoost = 5;
    //Values for boosts (Buff + debuff, multiplicative)
    //float damageBoostMultiplier = 2;
    //float heatPenalty = 2;
    //float aoeRangeBoost = 1.5f;
    //float burstRatePenalty = 0.66f;

    ModuleStatsTracker moduleStatsTracker;
    protected int weaponGenomeID = -1;

    // ===== Metric Tracking Fields =====
    private float totalDamageDealt = 0f;
    private float totalPowerCost = 0f;
    private float totalHeatGenerated = 0f;
    private int totalShotsFired = 0;
    private int totalHits = 0;
    private float activeFireTime = 0f;

    void Start()
    {
        OnStart();
    }
    protected virtual void OnStart()
    {
        chargeUpTimeAdjusted = weapon_Genome.chargeUpTime;
        fireRateAdjusted = weapon_Genome.fireRate;
        projectileSpeedAdjusted = weapon_Genome.projectileSpeed;
        powerCostAdjusted = weapon_Genome.powerCost;
        damageAdjusted = weapon_Genome.baseDamage;

        chargeUpTimer = chargeUpTimeAdjusted;
        shoot_Timer = 1 / Mathf.Max(fireRateAdjusted, 0.001f);
        bullets_Left_In_Burst = 0;
    }

    public void NewTarget(Transform target)
    {
        this.target = target;
    }

    public abstract Transform FindNewTarget();

    void Update()
    {
        if (isOverHeated)
        {
            currentHeat -= Time.deltaTime * weapon_Genome.heatDissipation;
            currentHeat = Mathf.Max(currentHeat, 0);
        }
        else
        {
            HandleShooting();
            
            // Track active firing time
            if (target != null && target.gameObject.activeSelf && bullets_Left_In_Burst > 0)
            {
                activeFireTime += Time.deltaTime;
            }
        }

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
            return;

        if (!shipHealthScript.ConsumePower(powerCostAdjusted * Time.deltaTime))
            return;

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

        cooldown_Timer = weapon_Genome.cooldownTime;
        if (activeBoost == WeaponBoost.BurstRate)
            cooldown_Timer -= burstRateBoost * boostTier;
        //if (activeBoost == WeaponBoost.BiggerAOELessBurstRate)
        //    cooldown_Timer *= burstRatePenalty;

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
                possibleAngle = Mathf.Max(possibleAngle - spreadAngleBoost * boostTier, 0);
            float spreadAngle = Random.Range(-possibleAngle, possibleAngle);
            rotation = Quaternion.Euler(0, 0, targetAngle + spreadAngle);
        }
        else
            rotation = Quaternion.Euler(0, 0, targetAngle);

        GameObject projectile_Instance = Instantiate(projectilePrefab, transform.position, rotation);
        int damage = damageAdjusted;
        float aoeRadius = weapon_Genome.aoeRadius;
        if (activeBoost == WeaponBoost.Damage)
            damage += Mathf.RoundToInt(damageBoostAdditive * boostTier);
        //if (activeBoost == WeaponBoost.MoreDamageMoreHeat)
        //    damage = Mathf.RoundToInt(damage * damageBoostMultiplier);
        //if (activeBoost == WeaponBoost.BiggerAOELessBurstRate)
        //aoeRadius *= aoeRangeBoost;

        if (moduleStatsTracker != null)
            projectile_Instance.GetComponent<Projectile>().SetInitialValues(damage, projectileSpeedAdjusted, weapon_Genome.range, aoeRadius, weapon_Genome.statusEffectType, weapon_Genome.statusEffectStrength, opponentTag, moduleStatsTracker);
        else if(weaponGenomeID != -1)
            projectile_Instance.GetComponent<Projectile>().SetInitialValues(damage, projectileSpeedAdjusted, weapon_Genome.range, aoeRadius, weapon_Genome.statusEffectType, weapon_Genome.statusEffectStrength, opponentTag,weaponGenomeID);
        else
                    projectile_Instance.GetComponent<Projectile>().SetInitialValues(damage, projectileSpeedAdjusted, weapon_Genome.range, aoeRadius, weapon_Genome.statusEffectType, weapon_Genome.statusEffectStrength, opponentTag);

        // ===== Track Metrics =====
        totalShotsFired++;
        totalPowerCost += powerCostAdjusted;
        
        bullets_Left_In_Burst--;
        shoot_Timer = 1 / Mathf.Max(fireRateAdjusted, 0.001f);
        int heatIncrease = weapon_Genome.heatPerShot;
        if (activeBoost == WeaponBoost.LowerHeatGeneration)
            heatIncrease -= Mathf.RoundToInt(heatBoost * boostTier);
        //if (activeBoost == WeaponBoost.MoreDamageMoreHeat)
        //    heatIncrease = Mathf.RoundToInt(heatIncrease * heatPenalty);
        
        totalHeatGenerated += heatIncrease;
        currentHeat += heatIncrease;

        //Rotates weapon to point in shooting direction
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }

    public void AddTracker(ModuleStatsTracker moduleStatsTracker)
    {
        this.moduleStatsTracker = moduleStatsTracker;
    }

    /// <summary>
    /// Called by Projectile when it successfully hits a target.
    /// Updates weapon metrics for hit ratio and damage dealt.
    /// </summary>
    public void OnProjectileHit(int damage)
    {
        totalDamageDealt += damage;
        totalHits++;
    }

    /// <summary>
    /// Register all accumulated metrics with the tracker.
    /// Should be called at end of battle/wave.
    /// </summary>
    public void RegisterBattleMetrics(float battleDuration)
    {
        if (tracker == null)
            return;

        // Register damage efficiency
        tracker.RegisterDamageEfficiency(
            totalDamageDealt,
            totalPowerCost > 0 ? totalPowerCost : 1f
        );

        // Register heat management efficiency
        tracker.RegisterHeatManagementEfficiency(
            totalDamageDealt,
            totalHeatGenerated > 0 ? totalHeatGenerated : 1f
        );

        // Register hit ratio
        tracker.RegisterHitRatio(
            totalHits,
            totalShotsFired > 0 ? totalShotsFired : 1f
        );

        // Register effectiveness per cycle
        float fireRateCycle = totalShotsFired > 0 ? 
            (totalShotsFired / weapon_Genome.fireRate) : 1f;
        tracker.RegisterEffectivenessPerCycle(
            totalDamageDealt,
            fireRateCycle,
            weapon_Genome.cooldownTime
        );

        // Register targeting time efficiency
        tracker.RegisterTargetingTimeEfficiency(
            activeFireTime,
            battleDuration > 0 ? battleDuration : 1f
        );
    }

    /// <summary>
    /// Reset all metric tracking for a new battle.
    /// </summary>
    public void ResetMetrics()
    {
        totalDamageDealt = 0f;
        totalPowerCost = 0f;
        totalHeatGenerated = 0f;
        totalShotsFired = 0;
        totalHits = 0;
        activeFireTime = 0f;
    }
}

public enum WeaponBoost
{
    None,
    Damage,
    BurstRate,
    Accuracy,
    LowerHeatGeneration
}
