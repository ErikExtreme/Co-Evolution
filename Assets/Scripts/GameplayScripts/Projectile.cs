using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage;
    private float aoeRadius;
    private float lifeTime;

    public void SetInitialValues(int damage, float velocity, float range, float aoeRadius)
    {
        this.damage = damage;
        gameObject.GetComponent<Rigidbody2D>().linearVelocity = transform.up * velocity;
        lifeTime = range / velocity;
        this.aoeRadius = aoeRadius;
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Asteroid"))
            return;

        collision.gameObject.GetComponent<Asteroid>().TakeDamage(damage);

        var Results = Physics2D.OverlapCircleAll(transform.position, aoeRadius);
        foreach (var aoeCollision in Results)
        {
            if (!aoeCollision.gameObject.CompareTag("Asteroid"))
                continue;

            aoeCollision.gameObject.GetComponent<Asteroid>().TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
