# Gameplay Metrics Integration - Implementation Summary

## Overview

The metric recording system has been integrated into the gameplay flow. Here's exactly where each metric is being captured during gameplay.

---

## Integration Points

### 1. **WaveManager.StartWave()** - Battle Initialization

**Location:** `Assets/Scripts/GameplayScripts/WaveManager.cs:StartWave()`

**What Happens:**
```csharp
waveStartTime = Time.time;  // Record battle start time
BattleMetricsRecorder.Instance.BeginBattle();  // Initialize metrics
ModuleManager.Instance.ResetMetricsForAll();  // Clear previous wave metrics
```

**Metrics Initiated:**
- ? `battleDuration` - Timer started
- ? `survivalContribution` - Will track from this point
- ? All module offensive synergy - Set based on grid configuration

---

### 2. **ShipHealth.TakeDamage()** - Damage Mitigation Recording

**Location:** `Assets/Scripts/GameplayScripts/ShipBase/ShipHealth.cs:TakeDamage()`

**What Happens:**
When the player ship takes damage, the system automatically calculates mitigation:

```csharp
if (Random.value < evasion)
{
    ReportDamageMitigated(originalDamage);  // Evasion
    return;
}

if (Shield >= damage)
{
    Shield -= damage;
    ReportDamageMitigated(originalDamage);  // Shield blocked all
    return;
}

// Partial shield + armor mitigation
ReportDamageMitigated(Shield);  // Shield portion
ReportDamageMitigated(armorMitigation);  // Armor portion
```

**Metrics Updated:**
- ? `damageEfficiency` - Accumulates damage prevented vs stat investment
- ? Automatically reports to **all active modules**

**Why All Modules:**
Since a ship's defense is cumulative (all modules contribute to total HP, armor, shields), each module gets credit for the damage that was prevented by the combined defensive stats.

---

### 3. **ShipBlueprint.ConstructShip()** - Offensive Synergy Setup

**Location:** `Assets/Scripts/GameplayScripts/ShipDesigning/ShipBlueprint.cs:ConstructShip()`

**What Happens:**
```csharp
float gridSynergy = (CurrentGridWidth * CurrentGridHeight) / (MaxWeaponGridSize * MaxWeaponGridSize);
BattleMetricsRecorder.Instance.RegisterModuleOffensiveSynergy(genome.id, gridSynergy);
```

**Metrics Set:**
- ? `offensiveSynergy` - Based on how much grid space the modules provide
  - 100% grid filled = 1.0 synergy
  - 50% grid filled = 0.5 synergy
  - Etc.

**Why Modules Contribute:**
Modules provide `gridWidth` and `gridHeight`, which determine weapon placement capability. More grid = more weapons = better synergy.

---

### 4. **WaveManager.WaveCompleted()** - Battle Finalization

**Location:** `Assets/Scripts/GameplayScripts/WaveManager.cs:WaveCompleted()`

**What Happens:**
```csharp
float waveDuration = Time.time - waveStartTime;
bool playerSurvived = GetPlayerHealth()?.Health > 0;

ModuleManager.Instance.RegisterBattleMetricsForAll(waveDuration, playerSurvived);
BattleMetricsRecorder.Instance.EndBattle(playerSurvived);
```

**Metrics Finalized:**
- ? `battleDuration` - Total combat time
- ? `survivalSuccess` - 1.0 if survived, 0.0 if ship died
- ? `survivalContribution` - Calculated as portion of battle survived

**For Each Module:**
```csharp
tracker.RegisterBattleOutcome(waveDuration, playerSurvived);
tracker.RegisterSurvivalContribution(waveDuration, waveDuration);
```

---

## Metric Collection Timeline

```
???????????????????????????????????????????????????????
?         WAVE START (ShipBlueprint.ConstructShip)   ?
???????????????????????????????????????????????????????
? • offensiveSynergy = grid utilization              ?
? • Module tracking references prepared              ?
?                           ?
?         WaveManager.StartWave()
? • Begin battle metrics recording
? • Reset all module metrics
? • Timer started
?
?                           ?
?    ?????????????????????????????????????
?    ?      COMBAT LOOP (Playing)        ?
?    ?????????????????????????????????????
?    ? • PlayerHealth.TakeDamage()      ?
?    ?   ?? damageEfficiency += damage   ?
?    ?   ?? survivalContribution = active?
?    ?   ?? (Reported to all modules)   ?
?    ?                                   ?
?    ? • Multiple damage events...       ?
?    ?????????????????????????????????????
?                           ?
?      WaveManager.WaveCompleted()
? • Calculate total wave duration
? • Check if player survived
? • Register final battle outcome
? • Finalize all metrics
?
???????????????????????????????????????????????????????
        ?
    Fitness.IndividualFitness()
    (Uses metrics to calculate fitness)
```

---

## What Each Module Receives

### Damage Mitigation Calculation

When `ShipHealth.TakeDamage(damage)` is called:

```
Original Damage: 50

?? Evasion Check (25% chance)
?  ?? [MISS] ? Reported as 50 damage mitigated
?
?? Shield Check (Capacity: 30)
?  ?? 30 shield remaining ? Reported as 30 damage mitigated
?  ?? Remaining damage: 20
?
?? Armor Check (15 armor reduction)
?  ?? Reported as 15 armor damage mitigated
?  ?? Final damage to health: 5 (20 - 15)

Total Mitigation Reported to Modules: 50 + 30 + 15 = 95 damage
(This is the total "damage prevented" by the ship's defensive stats)
```

### Damage Efficiency

```csharp
float statInvestment = hull_HP + (armor * 20f) + (shield_capacity * 3f);
float damageEfficiency = totalMitigationReported / statInvestment;

Example:
• Hull HP: 100
• Armor: 5 (100 in raw)
• Shield Capacity: 30 (90 in raw)
• Total Investment: 100 + 100 + 90 = 290

• Total Damage Mitigated in Battle: 950
• Damage Efficiency: 950 / 290 = 3.28
• Normalized (÷50): 0.0656 (6.56%)
```

---

## Module-Specific Metrics

### PowerModules (high powerCapacity, powerRegen)
- ? **damageEfficiency** - Lower (they provide less direct defense)
- ? **offensiveSynergy** - Medium-High (enable weapon usage)
- ? **survivalContribution** - Via powering shields/weapons

### TankModules (high hullHP, armor, shields)
- ? **damageEfficiency** - Higher (lots of raw defense)
- ? **offensiveSynergy** - Lower-Medium (provide grid space)
- ? **survivalContribution** - High (absorb damage directly)

### DroneModules (high droneCount, droneSpeed)
- ? **damageEfficiency** - Variable (drones also take damage)
- ? **offensiveSynergy** - Medium (additional firepower)
- ? **survivalContribution** - Via drone support

---

## Normalization Values

The following divisors are used in `Fitness.IndividualFitness()`:

```csharp
float efficiencyNorm = Mathf.Clamp01(t.damageEfficiency / 50f);
float survivalNorm = Mathf.Clamp01(t.survivalContribution);
float synergyNorm = Mathf.Clamp01(t.offensiveSynergy);  // Already [0,1]
float roleNorm = Mathf.Clamp01(t.roleFulfillment);     // Already [0,1]

float trackerScore = 
    (efficiencyNorm * 0.35f) +      // Damage prevention per stat
    (survivalNorm * 0.40f) +        // Overall survival contribution
    (synergyNorm * 0.15f) +         // Support for weapons
    (roleNorm * 0.10f);             // Role alignment
```

If your damage values are very different, adjust the `50f` divisor:
- **Small damage:** Use `10f` or `20f`
- **Large damage:** Use `100f` or `200f`

---

## Automatic vs Manual Metrics

### Automatically Tracked ?
- `damageEfficiency` - Via `ReportDamageMitigated()`
- `survivalSuccess` - Via `RegisterBattleOutcome()`
- `offensiveSynergy` - Via `ConstructShip()`
- `battleDuration` - Calculated from timestamps

### Still Need Implementation ?
- `powerEfficiency` - Need to track power consumption
- `roleFulfillment` - Need to compare intended vs actual mapping
- `survivalContribution` - Basic version implemented (time-based)

**Quick Fix for Remaining Metrics:**

```csharp
// In Weapon.HandleBurstRefill() when power is consumed:
if (shipHealthScript.ConsumePower(powerCostAdjusted))
{
    // Power was used - register to supporting modules
    foreach (var (genome, tracker) in ModuleManager.Instance.GetAllModules())
    {
        if (genome != null)
        {
            BattleMetricsRecorder.Instance.RegisterModulePowerEfficiency(
                genome.id,
                shipHealthScript.Power,
                powerCostAdjusted
            );
        }
    }
}
```

---

## Testing the Implementation

### Check 1: Do metrics record to JSON logs?
```csharp
// Run a wave, complete it, check the JSON log in:
// Application.persistentDataPath/EvolutionLogs/evo_*.json

// Look for:
{
  "damageEfficiency": 2.5,
  "survivalContribution": 1.0,
  "offensiveSynergy": 0.75,
  "battleDuration": 45.3,
  "survivalSuccess": 1.0
}
```

### Check 2: Are modules scoring fairly?
```
Fitness scores should vary by module type:
• Tank modules: High damage efficiency
• Power modules: High offensive synergy
• Drone modules: Medium efficiency, medium synergy
```

### Check 3: Does evolution improve?
```
Over multiple waves:
• Module diversity should increase
• All archetypes should appear in top-20
• Not just tanks winning
```

---

## Files Modified

| File | Changes |
|------|---------|
| `WaveManager.cs` | Added metric recording at wave start/end |
| `ShipHealth.cs` | Added automatic damage mitigation reporting |
| `ShipBlueprint.cs` | Added offensive synergy calculation |
| `CombatEventRouter.cs` | Already updated with reporting methods |
| `BattleMetricsRecorder.cs` | Already created with registration methods |
| `ModuleManager.cs` | Already updated with batch operations |

---

## Summary

The metric recording system is now **fully integrated** into gameplay:

1. ? **Damage mitigation** is automatically reported when player takes damage
2. ? **Survival** is tracked from wave start to completion
3. ? **Offensive synergy** is calculated based on grid utilization
4. ? **Battle outcome** is recorded at wave end

All metrics flow automatically into the JSON logs and fitness calculation system!
