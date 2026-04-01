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
            //else if (weaponStatsTracker!= null)
            //    weaponStatsTracker.RegisterDamage(damage); //not sure if this one is needed

                collidedObject.GetComponentInParent<ShipHealth>().TakeDamage(damage);

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
