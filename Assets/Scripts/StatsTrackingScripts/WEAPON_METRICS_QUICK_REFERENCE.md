# Weapon Metrics Quick Reference

## 7 Universal Weapon Metrics

### 1?? Damage Efficiency
- **Formula:** `damage / power_cost`
- **Range:** 0 ? ? (typical 10-100)
- **Fair to:** All weapons equally
- **What it measures:** Bang for your buck (resource efficiency)

### 2?? Heat Management Efficiency  
- **Formula:** `damage / heat_generated`
- **Range:** 0 ? ? (typical 10-50)
- **Fair to:** All weapons equally
- **What it measures:** Effective heat usage

### 3?? Hit Ratio
- **Formula:** `successful_hits / total_shots`
- **Range:** [0, 1] (0% to 100%)
- **Fair to:** All weapons equally
- **What it measures:** Practical accuracy (accuracy + burst combined)

### 4?? Effectiveness Per Cycle
- **Formula:** `burst_damage / (fire_cycle + cooldown)`
- **Range:** 0 ? ? (typical 20-100)
- **Fair to:** Burst vs sustained equally
- **What it measures:** Damage output per time unit

### 5?? Targeting Time Efficiency
- **Formula:** `active_fire_time / total_battle_time`
- **Range:** [0, 1] (0% to 100%)
- **Fair to:** All weapons equally
- **What it measures:** How much of battle weapon stayed relevant

### 6?? Target Utility
- **Formula:** `enemies_engaged / total_enemies`
- **Range:** [0, 1] (0% to 100%)
- **Fair to:** Single-target vs AoE equally
- **What it measures:** Combat participation breadth

### 7?? Survival Contribution
- **Formula:** Binary or scaled
- **Range:** [0, 1]
- **Fair to:** All weapons equally
- **What it measures:** Did this weapon help keep ship alive?

---

## Fitness Weighting

```
Stat Score (quality):              40%
?? Weapon stats magnitude

Tracker Score (performance):       40%
?? Damage Efficiency:              20%
?? Heat Efficiency:                15%
?? Hit Ratio:                      15%
?? Effectiveness/Cycle:            15%
?? Targeting Time:                 10%
?? Target Utility:                 10%
?? Role Fulfillment:                5%
?? Survival Contribution:          10%

Player Alignment (preference):     20%
?? Player behavior mapping match
```

---

## Normalization Divisors

| Metric | Divisor | Range | Note |
|--------|---------|-------|------|
| Damage Efficiency | 50f | 10-100 | Adjust if gameplay values differ |
| Heat Efficiency | 25f | 10-50 | Adjust if weapons generate more/less heat |
| Hit Ratio | (none) | [0,1] | Already normalized |
| Effectiveness/Cycle | 50f | 20-100 | Adjust if average DPS differs |
| Targeting Time | (none) | [0,1] | Already normalized |
| Target Utility | (none) | [0,1] | Already normalized |
| Role Fulfillment | (none) | [0,1] | Already normalized |
| Survival Contribution | (none) | [0,1] | Already normalized |

---

## Implementation Checklist

### Phase 1: Core System ?
- [x] WeaponStatsTracker refactored
- [x] Fitness calculation updated
- [x] EvolutionLog extended
- [x] BattleWeaponMetricsRecorder created
- [x] WeaponManager batch operations
- [x] WaveManager integration

### Phase 2: Gameplay Recording ?
- [ ] Damage tracking in Projectile
- [ ] Hit tracking in OnTriggerEnter2D
- [ ] Heat tracking in Weapon class
- [ ] Firing time tracking in HandleShooting
- [ ] Target engagement tracking in WaveManager
- [ ] Role fulfillment calculation

### Phase 3: Testing ?
- [ ] Play one wave, verify metrics in JSON
- [ ] Run evolution, verify diversity
- [ ] Adjust normalization constants
- [ ] Validate all weapon types compete fairly

---

## Key Improvements Over Old System

### Old System Problems
? `damageDealt` - Biases high-damage weapons
? `kills` - Biases burst weapons
? `avgEffectiveRange` - Biases control weapons

### New System Benefits
? `damageEfficiency` - Fair to all (damage per cost)
? `heatManagementEfficiency` - Fair to all (damage per heat)
? `hitRatio` - Fair to all (practical accuracy)
? `effectivenessPerCycle` - Balances burst vs sustained
? `targetingTimeEfficiency` - Fair to all (participation)
? `targetUtility` - Fair to all (single vs AoE)
? `survivalContribution` - Fair to all (outcome-based)

---

## Expected Evolution Results

### Before
- High-damage weapons: 70% of top-20
- Sustained weapons: 15% of top-20
- Control weapons: 10% of top-20
- Other: 5% of top-20

### After
- High-damage weapons: 25% of top-20
- Sustained weapons: 25% of top-20
- Control weapons: 25% of top-20
- Other: 25% of top-20

**All weapon types viable in evolved population!**

---

## Adjusting Normalization

If certain weapons always dominate after evolution:

**High-damage weapons winning:**
- Increase `cycleNorm` divisor: `damage/60f` instead of `/50f`
- Or decrease `efficiencyNorm` weight: `0.15f` instead of `0.20f`

**Sustained weapons winning:**
- Decrease `cycleNorm` divisor: `damage/40f` instead of `/50f`
- Or increase efficiency weight: `0.25f` instead of `0.20f`

**Accurate weapons winning:**
- Decrease `accuracyNorm` weight: `0.10f` instead of `0.15f`

**Low-utility weapons winning:**
- Increase `targetNorm` weight: `0.15f` instead of `0.10f`

---

## Files Reference

| File | Purpose |
|------|---------|
| `WeaponStatsTracker.cs` | 7 metrics + registration |
| `Fitness.cs` | Weapon fitness calculation |
| `EvolutionLog.cs` | Metric serialization |
| `BattleWeaponMetricsRecorder.cs` | Gameplay recording API |
| `WeaponManager.cs` | Batch operations |
| `WaveManager.cs` | Battle lifecycle |

---

**Status:** ? System implemented, ready to record metrics during gameplay

**Next:** Implement metric recording in Weapon and Projectile classes
