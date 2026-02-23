using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponGenome weapon_Genome;
    public WeaponStatsTracker tracker;

    public ShipHealth shipHealthScript;
    public PlayerShip playerShip;

    [SerializeField] GameObject projectilePrefab;

    float shoot_Timer;
    int bullets_Left_In_Burst;
    float cooldown_Timer;
    float currentHeat;
    int maxHeat = 100;
    bool overHeated;

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

        shoot_Timer = 1 / weapon_Genome.fireRate;
        bullets_Left_In_Burst = 0;

        playerShip = transform.GetComponentInParent<PlayerShip>();
        playerShip.targetSelection += NewTarget;
    }

    public void NewTarget(Transform target)
    {
        this.target = target;
    }

    public Transform FindNewTarget()
    {
        return EnemyManager.Instance.GetClosestEnemy(transform.position);
    }

    void Update()
    {
        if (!overHeated)
        {
            HandleShooting();
        }

        currentHeat -= Time.deltaTime * weapon_Genome.heatDissipation;//Only dissipate heat while overheated?, slower dissipation when not overheated?
        currentHeat = Mathf.Max(currentHeat, 0);

        if (currentHeat >= maxHeat)
            overHeated = true;
        else if (currentHeat <= 0)
            overHeated = false;
    }

    private void HandleShooting()
    {
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
        projectile_Instance.GetComponent<Projectile>().SetInitialValues(weapon_Genome.baseDamage, weapon_Genome.projectileSpeed, weapon_Genome.range, weapon_Genome.aoeRadius);


        bullets_Left_In_Burst--;
        cooldown_Timer = weapon_Genome.cooldownTime;
        currentHeat += weapon_Genome.heatPerShot;
    }
}
