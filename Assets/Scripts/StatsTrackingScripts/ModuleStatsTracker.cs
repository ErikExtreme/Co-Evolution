using UnityEngine;

public class ModuleStatsTracker
{
    public float timeEquipped;
    public float damageAvoided;
    public float powerSaved;
    public float heatReduced;

    public void UpdateEquipped(float dt)
    {
        timeEquipped += dt;
    }

    public void RegisterDamageAvoided(float amount)
    {
        damageAvoided += amount;
    }

    public void RegisterPowerSaved(float amount)
    {
        powerSaved += amount;
    }

    public void RegisterHeatReduced(float amount)
    {
        heatReduced += amount;
    }
}
