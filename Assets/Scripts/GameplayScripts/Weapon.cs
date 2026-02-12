using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponGenome weapon_Genome;
    public WeaponStatsTracker tracker;

    public ShipHealth shipHealth;


    [SerializeField] GameObject projectilePrefab;

    float shoot_Timer;
    int bullets_Left_In_Burst;
    float cooldown_Timer;

    void Start()
    {
        weapon_Genome = new WeaponGenome();
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

        weapon_Genome.aoeRadius = 1f;

        shoot_Timer = 1 / weapon_Genome.fireRate;
        bullets_Left_In_Burst = 0;
    }

    void Update()
    {
        if (bullets_Left_In_Burst <= 0)
        {
            shoot_Timer -= Time.deltaTime;
            if (shoot_Timer <= 0)
            {
                shoot_Timer = 1 / weapon_Genome.fireRate;

                if (shipHealth.ConsumePower(weapon_Genome.powerCost))
                    bullets_Left_In_Burst = weapon_Genome.burstSize;
            }
        }
        else
        {
            cooldown_Timer -= Time.deltaTime;
            if (cooldown_Timer <= 0)
            {
                bullets_Left_In_Burst--;
                cooldown_Timer = weapon_Genome.cooldownTime;

                float spreadAngle = Random.Range(-weapon_Genome.spreadAngle, weapon_Genome.spreadAngle);
                Quaternion rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + spreadAngle);

                GameObject projectile_Instance = Instantiate(projectilePrefab, transform.position, rotation);
                projectile_Instance.GetComponent<Projectile>().SetInitialValues(weapon_Genome.baseDamage, weapon_Genome.projectileSpeed, weapon_Genome.range,weapon_Genome.aoeRadius);

            }
        }

    }
}
