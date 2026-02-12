using UnityEngine;

public class WeaponStatsTracker
{
    public float timeEquipped;
    public float damageDealt;
    public int kills;
    public float avgEffectiveRange;

    float smoothing = 0.1f;

    public void UpdateEquipped(float dt)
    {
        timeEquipped += dt;
    }

    public void RegisterDamage(float dmg, float distance)
    {
        damageDealt += dmg;
        avgEffectiveRange = Mathf.Lerp(avgEffectiveRange, distance, smoothing);
    }

    public void RegisterKill()
    {
        kills++;
    }
}
