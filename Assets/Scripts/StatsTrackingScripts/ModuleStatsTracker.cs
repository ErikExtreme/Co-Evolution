using UnityEngine;

public class ModuleStatsTracker
{
    public float timeEquipped;
    public float damageTaken;
    public float droneDamageDealt;
    public float droneDamageTaken;
    public float distanceMoved;

    public void UpdateEquipped(float dt)
    {
        timeEquipped += dt;
    }

    public void RegisterDamageTaken(float amount)
    {
        damageTaken += amount;
    }

    public void RegisterDroneDamageDealt(float amount)
    {
        droneDamageDealt += amount;
    }
    public void RegisterDroneDamageTaken(float amount)
    {
        droneDamageTaken += amount;
    }
    public void RegisterDistanceMoved(float amount)
    {
        Debug.Log("moved " + amount);
        distanceMoved += amount;
    }
}
