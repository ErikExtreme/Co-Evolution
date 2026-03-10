using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponGenome weapon_Genome;
    public WeaponStatsTracker tracker;

    public ShipHealth shipHealthScript;

    [SerializeField] GameObject projectilePrefab;
    protected string opponentTag;

    float chargeUpTimer;

    float currentHeat;
    int maxHeat = 100;
    bool isOverHeated;

    int bullets_Left_In_Burst;
    float cooldown_Timer;

    float shoot_Timer;


    private Transform target;

    void Start()
    {
        /*weapon_Genome = new WeaponGenome();
        //weapon_Genome = GlobalSettings.RandomWeaponGenome();

        //Temporary(?) default values
        weapon_Genome.baseDamage = 1;
        weapon_Genome.burstSize = 3;
        weapon_Genome.fireRate = 1f;
        weapon_Genome.cooldownTime = 0.3f;
        weapon_Genome.projectileSpeed = 2;
        //weapon_Genome.accuracy   unimplemented
        weapon_Genome.spreadAngle = 5;
        weapon_Genome.range = 5;

        weapon_Genome.powerCost = 1;

        weapon_Genome.aoeRadius = 1f;*/

        OnStart();
    }
    protected virtual void OnStart()
    {
        chargeUpTimer = weapon_Genome.chargeUpTime;
        shoot_Timer = 1 / weapon_Genome.fireRate;
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
            chargeUpTimer = weapon_Genome.chargeUpTime;
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

        cooldown_Timer -= Time.deltaTime;
        if (cooldown_Timer <= 0)
            FireBullet();
    }
    private void HandleBurstRefill()
    {
        shoot_Timer -= Time.deltaTime;
        if (shoot_Timer > 0)
            return;
        shoot_Timer = 1 / weapon_Genome.fireRate;

        if (shipHealthScript.ConsumePower(weapon_Genome.powerCost))
            bullets_Left_In_Burst = weapon_Genome.burstSize;
    }
    private void FireBullet()
    {
        Vector2 targetDirection = ((Vector2)target.position - (Vector2)transform.position).normalized;
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;

        float spreadAngle = Random.Range(-weapon_Genome.spreadAngle, weapon_Genome.spreadAngle);
        Quaternion rotation = Quaternion.Euler(0, 0, targetAngle + spreadAngle);


        GameObject projectile_Instance = Instantiate(projectilePrefab, transform.position, rotation);
        projectile_Instance.GetComponent<Projectile>().SetInitialValues(weapon_Genome.baseDamage, weapon_Genome.projectileSpeed, weapon_Genome.range, weapon_Genome.aoeRadius, weapon_Genome.statusEffectType, weapon_Genome.statusEffectStrength, opponentTag);


        bullets_Left_In_Burst--;
        cooldown_Timer = weapon_Genome.cooldownTime;
        currentHeat += weapon_Genome.heatPerShot;

        //Rotates weapon to point in shooting direction
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }
}
