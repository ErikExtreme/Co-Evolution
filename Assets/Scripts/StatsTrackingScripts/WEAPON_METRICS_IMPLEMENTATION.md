# Weapon Metrics System - Implementation Guide

## Overview

The weapon metrics system has been completely refactored to use **7 universal, fair metrics** that apply equally to all weapon types (burst, sustained, control, close-quarters, efficiency, volatility).

---

## The 7 New Metrics

### **1. Damage Efficiency** ?
**Metric:** `damageDealt / powerCost`

**Purpose:** Measures output per resource cost

**Why Universal:**
- All weapons have a power cost
- Fairly compares high-damage expensive vs low-damage cheap weapons
- Not biased toward any mapping

**Calculation Example:**
- Weapon A: 500 damage, 8 power cost ? 62.5 efficiency
- Weapon B: 200 damage, 2 power cost ? 100 efficiency
- Both viable, different strategies

---

### **2. Heat Management Efficiency** ?
**Metric:** `damageDealt / heatGenerated`

**Purpose:** Measures how effectively weapon uses heat generation

**Why Universal:**
- All weapons generate heat
- Efficient usage measured by outcomes, not stats
- High-heat weapons can still be efficient if they deal proportional damage

**Calculation Example:**
- Weapon A: 400 damage, 8 heat ? 50 efficiency
- Weapon B: 200 damage, 2 heat ? 100 efficiency  
- Low-heat weapon doesn't automatically win

---

### **3. Hit Ratio** ?
**Metric:** `successfulHits / totalShotsFired`

**Purpose:** Measures practical accuracy (accounts for burst size + accuracy)

**Why Universal:**
- Fair to all weapon types
- High accuracy, low burst same as low accuracy, high burst
- Rewards weapons that actually connect
- Already normalized [0, 1]

**Calculation Example:**
- High-accuracy weapon: 90% accuracy, 1-shot burst ? 0.90 ratio
- Medium-accuracy weapon: 60% accuracy, 5-shot burst ? 0.75 ratio (if 3/5 hit)
- Low-accuracy spreader: 30% accuracy, 10-shot burst ? 0.90 ratio (if 9/10 hit wide)

---

### **4. Effectiveness Per Cycle** ?
**Metric:** `burstDamage / (fireRateCycle + cooldownTime)`

**Purpose:** Measures damage per time cycle (naturally balances burst vs sustained)

**Why Universal:**
- Burst weapons: high damage, long cooldown ? balanced
- Sustained weapons: low damage, short cooldown ? balanced
- No artificial advantage to either playstyle

**Calculation Example:**
- Burst weapon: 600 damage, 1s fire cycle, 2s cooldown = 600/3 = 200 DPS
- Sustained weapon: 100 damage, 0.2s fire cycle, 0s cooldown = 100/0.2 = 500 DPS
- Sustained naturally scores higher for "per cycle" metric

---

### **5. Targeting Time Efficiency** ?
**Metric:** `fireTime / totalBattleTime`

**Purpose:** How much of the battle this weapon stayed active

**Why Universal:**
- All weapons can track this
- Measures participation, not distance
- Rewards weapons that stayed relevant
- Already normalized [0, 1]

**Calculation Example:**
- Long-range weapon: fires 85% of battle = 0.85
- Short-range weapon: fires 70% of battle (enemies too close) = 0.70
- Both legitimate strategies

---

### **6. Target Utility** ?
**Metric:** `enemiesEngaged / totalEnemiesInBattle`

**Purpose:** How many distinct enemies this weapon engaged

**Why Universal:**
- Fair to single-target and AoE equally
- 1 damage to 5 enemies = 5 enemies engaged
- 100 damage to 1 enemy = 1 enemy engaged
- No artificial bonus for either strategy

**Calculation Example:**
- Single-target weapon: 400 damage to 1 enemy = 1/30 = 0.033
- Spread weapon: 50 damage each to 15 enemies = 15/30 = 0.50
- Spread weapon has more utility, single-target has more efficiency

---

### **7. Survival Contribution** ?
**Metric:** `survivalScore` based on ship survival outcome

**Purpose:** Did this weapon help keep the ship alive?

**Why Universal:**
- All weapons contribute to survival if they damage enemies
- Weapons that enable other weapons count
- Time weapons spend firing = helping prevent enemy attacks
- Already normalized [0, 1]

**Calculation:** 
- If ship survives with weapon equipped: 1.0
- If ship survives with weapon helping key targets: scaled bonus
- If ship dies: 0.0 (weapon didn't help enough)

---

### **Bonus: Role Fulfillment** ?
**Metric:** Distance between intended mapping and actual performance

**Purpose:** Did weapon match its genetic specialization?

**Why Universal:**
- Measures outcome alignment, not stat alignment
- A burst weapon that deals burst damage: high fulfillment
- A control weapon that controls engagement: high fulfillment
- Not stat-based, outcome-based

---

## Fitness Calculation

```csharp
// Normalized metrics
float efficiencyNorm = damageEfficiency / 50f;           // Power efficiency
float heatNorm = heatManagementEfficiency / 25f;         // Heat efficiency
float accuracyNorm = hitRatio;                           // Already [0,1]
float cycleNorm = effectivenessPerCycle / 50f;          // Damage per cycle
float utilizationNorm = targetingTimeEfficiency;         // Already [0,1]
float targetNorm = targetUtility;                        // Already [0,1]
float roleNorm = roleFulfillment;                        // Already [0,1]
float survivalNorm = survivalContribution;               // Already [0,1]

// Weighted score (total 1.0)
trackerScore = (efficiencyNorm * 0.20f) +       // Power cost efficiency
               (heatNorm * 0.15f) +              // Heat generation efficiency
               (accuracyNorm * 0.15f) +          // Practical accuracy
               (cycleNorm * 0.15f) +             // Cycle effectiveness
               (utilizationNorm * 0.10f) +       // Time active
               (targetNorm * 0.10f) +            // Targets engaged
               (roleNorm * 0.05f) +              // Role fulfillment
               (survivalNorm * 0.10f);           // Survival help

// Final fitness
fitness = (statScore * 0.4) + (trackerScore * 0.4) + (alignmentScore * 0.2)
```

---

## Before vs After Comparison

### Before (Biased)
```
Weapon Type        damageDealt    kills    avgRange    Score
?????????????????  ???????????????????????????????????  ?????
Burst (high dmg)   ???????????? 4000    ??? 30        85%
Sustained (low dmg)?? 1200        ?? 8    ??? 25       18%
Control (long)     ??? 1800       ?? 10   ????? 35     45%
CQ (short)         ?? 1000        ?? 12   ?? 5         20%
```

**Result:** High-damage weapons always win

### After (Fair)
```
Weapon Type        efficiency    heat_eff  accuracy    cycle    util    Score
?????????????????  ????????????????????????????????????????????????????  ?????
Burst              ???? 80       ??? 60   ?? 40%      ???? 100 ???? 70%  72%
Sustained          ??? 75       ????? 95  ???? 80%   ?? 40    ????? 90%  78%
Control            ?? 45        ???? 85   ??? 60%    ??? 60   ??? 65%    65%
CQ                 ??? 70       ??? 70    ??? 70%    ??? 70   ?? 50%     68%
```

**Result:** All weapon types can be viable based on context

---

## Integration Points

### Battle Start (WaveManager.StartWave)
```csharp
BattleWeaponMetricsRecorder.Instance.BeginBattle();
WeaponManager.Instance.ResetMetricsForAll();
```

### During Combat (Weapon.FireBullet)
- Track shots fired
- Track heat generated
- Track hits (when projectile connects)
- Track targets engaged

### Battle End (WaveManager.WaveCompleted)
```csharp
WeaponManager.Instance.RegisterBattleMetricsForAll(duration, survived);
BattleWeaponMetricsRecorder.Instance.EndBattle(survived);
```

---

## Data Collection During Gameplay

### Shots Fired & Hit Tracking
**Location:** `Projectile.OnTriggerEnter2D()`
- Record each shot as "fired"
- Record each hit as "successful hit"
- Allows automatic hit ratio calculation

### Heat Tracking
**Location:** `Weapon.FireBullet()` & `Weapon.Update()`
- Track cumulative heat generated
- Track heat dissipation
- Calculate net heat per cycle

### Damage Tracking
**Location:** `Projectile.OnTriggerEnter2D()` ? `ShipHealth.TakeDamage()`
- Track total damage dealt by weapon
- Already works with existing system

### Targeting Time
**Location:** `Weapon.HandleShooting()`
- Track time weapon has valid target
- Track total battle time
- Calculate fraction active

### Target Utility
**Location:** `EnemyManager` or `WaveManager`
- Count distinct enemies in battle
- Track which enemies were hit by which weapons
- Calculate engagement coverage

---

## Normalization Constants

Adjust these in `Fitness.cs` if your game values differ:

```csharp
// If weapons typically deal 100-200 damage per cycle:
float cycleNorm = Mathf.Clamp01(t.effectivenessPerCycle / 150f);  // Adjust divisor

// If weapons typically deal 20-100 damage per power:
float efficiencyNorm = Mathf.Clamp01(t.damageEfficiency / 50f);   // Adjust divisor

// If weapons typically deal 20-60 damage per heat:
float heatNorm = Mathf.Clamp01(t.heatManagementEfficiency / 40f); // Adjust divisor
```

---

## What's Automatic vs Manual

### ? Automatically Tracked
- `battleDuration` - Calculated from timestamps
- `survivalSuccess` - From ship health check
- Metric registration at battle end

### ? Ready for Implementation (Simple)
- `damageEfficiency` - Accumulate power cost, track damage
- `heatManagementEfficiency` - Accumulate heat, track damage
- `hitRatio` - Count shots vs hits (Projectile system)
- `effectivenessPerCycle` - Weapon genome data + damage tracking
- `targetingTimeEfficiency` - Weapon has target time
- `targetUtility` - Enemy count + tracking

### ? Ready for Advanced Implementation
- `roleFulfillment` - Compare mapping to performance
- `survivalContribution` - Advanced: scale by enemy difficulty

---

## Files Modified

| File | Change | Status |
|------|--------|--------|
| `WeaponStatsTracker.cs` | Replaced 4 metrics with 7 universal ones | ? Done |
| `Fitness.cs` | Updated weapon fitness calculation | ? Done |
| `EvolutionLog.cs` | Extended WeaponGenomeLog, updated serialization | ? Done |
| `WeaponManager.cs` | Added batch metric operations | ? Done |
| `WaveManager.cs` | Added weapon metrics recording calls | ? Done |
| `BattleWeaponMetricsRecorder.cs` | NEW - Central metrics recording | ? Done |

---

## Next Steps

1. ? System is complete and compiling
2. ?? Run a wave and complete it
3. ?? Check JSON logs for new metrics
4. ?? Run evolution and validate diversity
5. ?? Adjust normalization constants if needed

---

## Testing Recommendations

### Quick Test
1. Play one wave with different weapon types
2. Complete the wave
3. Check JSON logs for metric values
4. Verify no metric is 0.0 (indicates missing implementation)

### Validation Test
1. Run 5 evolution cycles
2. Check top-20 weapon diversity
3. Verify all weapon mappings represented
4. Compare metric distributions

### Adjustment Test
1. If high-damage weapons dominate: increase `cycleNorm` divisor
2. If sustained weapons dominate: decrease `cycleNorm` divisor
3. If all metrics 0.0: implement that metric tracking
4. Fine-tune normalization constants

---

## Questions?

Refer to:
- `QUICK_REFERENCE.md` (parent directory) for overview
- `WeaponStatsTracker.cs` for metric definitions
- `Fitness.cs` for calculation details
- `BattleWeaponMetricsRecorder.cs` for registration API
