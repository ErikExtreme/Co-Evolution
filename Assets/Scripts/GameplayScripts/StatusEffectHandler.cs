using System.Collections.Generic;
using UnityEngine;

public class StatusEffectHandler : MonoBehaviour
{
    private List<StatusEffect> statusEffects;

    private ShipHealth healthScript;
    private ShipMovement movementScript;

    private float secondTimer = 0;
    void Start()
    {
        statusEffects = new List<StatusEffect>();

        healthScript = GetComponent<ShipHealth>();
        movementScript = GetComponent<ShipMovement>();
    }

    void Update()
    {
        statusEffects.ForEach(statusEffect => statusEffect.timeLeft -= Time.deltaTime);

        if (secondTimer >= 1)
        {
            foreach (var statusEffect in statusEffects)
            {
                if (statusEffect.timeLeft <= 0)
                {
                    StatusEffectExpired(statusEffect);
                    continue;
                }

                switch (statusEffect.type)
                {
                    case EffectType.Burn:
                        healthScript.TakeDamage((int)(5 * statusEffect.strength));//No clue what a resonable base damage would be, should it be based on the weapons damage?
                        break;
                    case EffectType.Slow:
                        //No active effect
                        break;
                    case EffectType.ArmorPierce:
                        //No active effect
                        break;
                    default:
                        break;
                }
            }
            statusEffects.RemoveAll(statusEffect =>  statusEffect.timeLeft <= 0);

            secondTimer = 0;
        }

        secondTimer += Time.deltaTime;
    }

    public void ApplyEffect(EffectType type, float strength)
    {
        if (type == EffectType.None)
            return;

        int existingEffectIndex = statusEffects.FindIndex(effect => effect.type == type);
        if (existingEffectIndex >= 0)
        {
            if (strength > statusEffects[existingEffectIndex].strength)
                statusEffects.RemoveAt(existingEffectIndex);
            else
                return;
        }

        statusEffects.Add(new StatusEffect(type, strength, 5));//5 is duration in seconds, dont know what to base it on, statusEffectStrength?

        switch (type)
        {
            case EffectType.Burn:
                //No passive effect
                break;
            case EffectType.Slow:
                movementScript.speedModifier = 1 - 0.5f * strength;//Converts 1-0 range to 0.5-1 range
                break;
            case EffectType.ArmorPierce:
                healthScript.armorReduction = 1 - 0.5f * strength;//Converts 1-0 range to 0.5-1 range
                break;
            default:
                break;
        }
    }
    private void StatusEffectExpired(StatusEffect statusEffect)
    {
        switch (statusEffect.type)
        {
            case EffectType.Burn:
                //No passive effect
                break;
            case EffectType.Slow:
                movementScript.speedModifier = 1;//Resets to base speed
                break;
            case EffectType.ArmorPierce:
                healthScript.armorReduction = 1;//Resets to base armor
                break;
            default:
                break;
        }
    }
}
class StatusEffect
{
    public EffectType type;
    public float strength;
    public float timeLeft;

    public StatusEffect(EffectType type, float strength, float timeLeft)
    {
        this.type = type;
        this.strength = strength;
        this.timeLeft = timeLeft;
    }
}
public enum EffectType
{
    None,
    Burn,
    Slow,
    ArmorPierce
}
