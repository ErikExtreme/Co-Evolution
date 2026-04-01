using UnityEngine;

public class ModuleStatsTracker
{
    public float timeEquipped;
    public float damageTaken;
    public float powerGenerated;
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

    public void RegisterPowerGenerated(float amount)
    {
        powerGenerated += amount;
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
        distanceMoved += amount;
    }
}
