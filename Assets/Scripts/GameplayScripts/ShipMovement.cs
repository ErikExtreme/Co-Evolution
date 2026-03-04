using UnityEngine;

public abstract class ShipMovement : MonoBehaviour
{
    //Stats
    protected float speed;
    protected float turnRate;
    protected float mass;
    protected float inertia;

    protected float angularDamping = 0.05f;


    protected Rigidbody2D rigidbodyThis;

    [System.NonSerialized] public float speedModifier = 1;
    public float Speed { get { return speed * speedModifier; } }

    void Start()
    {
        OnStart();
    }
    protected virtual void OnStart()
    {
        rigidbodyThis = GetComponent<Rigidbody2D>();
    }
}
