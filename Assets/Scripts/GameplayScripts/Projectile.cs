using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage;
    private float aoeRadius;
    private float lifeTime;

    private EffectType effectType;
    private float effectStrength;

    private string opponentTag;

    //WeaponStatsTracker weaponStatsTracker;
    ModuleStatsTracker moduleStatsTracker;
    int weaponGenomeID = -1;

    public void SetInitialValues(int damage, float velocity, float range, float aoeRadius, EffectType effectType, float effectStrength, string opponentTag)
    {
        this.damage = damage;
        gameObject.GetComponent<Rigidbody2D>().linearVelocity = transform.up * velocity;
        lifeTime = range / velocity;
        this.aoeRadius = aoeRadius;

        this.effectType = effectType;
        this.effectStrength = effectStrength;

        this.opponentTag = opponentTag;
    }
    public void SetInitialValues(int damage, float velocity, float range, float aoeRadius, EffectType effectType, float effectStrength, string opponentTag, ModuleStatsTracker moduleStatsTracker)
    {
        this.damage = damage;
        gameObject.GetComponent<Rigidbody2D>().linearVelocity = transform.up * velocity;
        lifeTime = range / velocity;
        this.aoeRadius = aoeRadius;

        this.effectType = effectType;
        this.effectStrength = effectStrength;

        this.opponentTag = opponentTag;

        this.moduleStatsTracker = moduleStatsTracker;
    }
    public void SetInitialValues(int damage, float velocity, float range, float aoeRadius, EffectType effectType, float effectStrength, string opponentTag, int weaponGenomeID)
    {
        this.damage = damage;
        gameObject.GetComponent<Rigidbody2D>().linearVelocity = transform.up * velocity;
        lifeTime = range / velocity;
        this.aoeRadius = aoeRadius;

        this.effectType = effectType;
        this.effectStrength = effectStrength;

        this.opponentTag = opponentTag;

        this.weaponGenomeID = weaponGenomeID;
    }
    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collidedObject = collision.gameObject;

        if (collidedObject.CompareTag(opponentTag))
        {
            if (moduleStatsTracker != null)
                moduleStatsTracker.RegisterDroneDamageDealt(damage);
            
            // Notify weapon of hit for metric tracking
            if (weaponGenomeID != -1)
            {
                Weapon weaponScript = FindWeaponScript(weaponGenomeID);
                if (weaponScript != null)
                {
                    weaponScript.OnProjectileHit(damage);
                }
            }

            if(collidedObject.GetComponentInParent<ShipHealth>().TakeDamage(damage))
            {
                // Enemy died - don't need to report kill anymore, metrics handled at battle end
            }

            if (effectType == EffectType.Burn)
                collidedObject.GetComponentInParent<StatusEffectHandler>().ApplyEffect(effectType, effectStrength * damage / 4);
            else
                collidedObject.GetComponentInParent<StatusEffectHandler>().ApplyEffect(effectType, effectStrength);

            AoeCollision(collidedObject);
            Destroy(gameObject);
        }
        else if (collidedObject.CompareTag("Drone") && opponentTag == "Player")
        {
            if (collidedObject.TryGetComponent<Drone>(out var droneScript))
            {
                droneScript.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Find the weapon script by genome ID for metric tracking.
    /// </summary>
    private Weapon FindWeaponScript(int genomeId)
    {
        Weapon[] allWeapons = FindObjectsOfType<Weapon>();
        foreach (var weapon in allWeapons)
        {
            if (weapon.weapon_Genome != null && weapon.weapon_Genome.id == genomeId)
            {
                return weapon;
            }
        }
        return null;
    }
    private void AoeCollision(GameObject hitObject)
    {
        var Results = Physics2D.OverlapCircleAll(transform.position, aoeRadius);
        foreach (var aoeCollision in Results)
        {
            if (aoeCollision == hitObject)
                continue;

            GameObject hitObjectAOE = aoeCollision.gameObject;

            if (hitObjectAOE.CompareTag(opponentTag))
            {
                hitObjectAOE.GetComponentInParent<ShipHealth>().TakeDamage(damage / 2);

                if (effectType == EffectType.Burn)
                    hitObjectAOE.GetComponentInParent<StatusEffectHandler>().ApplyEffect(effectType, effectStrength * damage / 10);
                else
                    hitObjectAOE.GetComponentInParent<StatusEffectHandler>().ApplyEffect(effectType, effectStrength);
            }
        }
    }
}
