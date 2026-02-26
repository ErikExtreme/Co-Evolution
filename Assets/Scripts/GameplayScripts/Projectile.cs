using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage;
    private float aoeRadius;
    private float lifeTime;

    private string opponentTag;

    public void SetInitialValues(int damage, float velocity, float range, float aoeRadius, string opponentTag)
    {
        this.damage = damage;
        gameObject.GetComponent<Rigidbody2D>().linearVelocity = transform.up * velocity;
        lifeTime = range / velocity;
        this.aoeRadius = aoeRadius;

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
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            collision.gameObject.GetComponent<Asteroid>().TakeDamage(damage);

            AoeCollision();
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag(opponentTag))
        {
            collision.gameObject.GetComponentInParent<ShipHealth>().TakeDamage(damage);

            AoeCollision();
            Destroy(gameObject);
        }

    }
    private void AoeCollision()
    {
        var Results = Physics2D.OverlapCircleAll(transform.position, aoeRadius);
        foreach (var aoeCollision in Results)
        {
            if (aoeCollision.gameObject.CompareTag(opponentTag))
                aoeCollision.gameObject.GetComponentInParent<ShipHealth>().TakeDamage(damage);

            if (aoeCollision.gameObject.CompareTag("Asteroid"))
                aoeCollision.gameObject.GetComponent<Asteroid>().TakeDamage(damage);
        }
    }
}
