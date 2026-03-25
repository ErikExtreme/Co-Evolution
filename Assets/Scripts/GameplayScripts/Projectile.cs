using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage;
    private float aoeRadius;
    private float lifeTime;

    private EffectType effectType;
    private float effectStrength;

    private string opponentTag;

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

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(opponentTag))
        {
            collision.gameObject.GetComponentInParent<ShipHealth>().TakeDamage(damage);
            if (effectType == EffectType.Burn)
                collision.gameObject.GetComponentInParent<StatusEffectHandler>().ApplyEffect(effectType, effectStrength * damage/4);
            else
                collision.gameObject.GetComponentInParent<StatusEffectHandler>().ApplyEffect(effectType, effectStrength);

            AoeCollision(collision.gameObject);
            Destroy(gameObject);
        }

    }
    private void AoeCollision(GameObject hitObject)
    {
        var Results = Physics2D.OverlapCircleAll(transform.position, aoeRadius);
        foreach (var aoeCollision in Results)
        {
            if (aoeCollision == hitObject)
                continue;

            if (aoeCollision.gameObject.CompareTag(opponentTag))
            {
                aoeCollision.gameObject.GetComponentInParent<ShipHealth>().TakeDamage(damage / 2);
                if (effectType == EffectType.Burn)
                    aoeCollision.gameObject.GetComponentInParent<StatusEffectHandler>().ApplyEffect(effectType, effectStrength * damage/3);
                else
                    aoeCollision.gameObject.GetComponentInParent<StatusEffectHandler>().ApplyEffect(effectType, effectStrength);
            }
        }
    }
}
