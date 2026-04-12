# ModuleStatsTracker Metrics System Overhaul

## Summary of Changes

This document outlines the complete overhaul of the `ModuleStatsTracker` system to use **universal, outcome-based metrics** that are fair across all ship module types.

---

## What Changed

### 1. **ModuleStatsTracker.cs** - New Universal Metrics

**Old Metrics (Deprecated):**
- `timeEquipped` - Meaningless for fitness (all modules stay equipped)
- `damageAvoided` - Biased toward tank builds (HP, armor, shields)
- `powerSaved` - Biased toward power-focused modules
- `heatReduced` - Biased toward heat dissipation modules

**New Metrics (Universal & Fair):**

| Metric | Purpose | Range | How to Register |
|--------|---------|-------|-----------------|
| `damageEfficiency` | Damage prevented per stat point invested | [0, ?) | `RegisterDamageEfficiency(damageAbsorbed, statInvestment)` |
| `survivalContribution` | Fraction of battle ship stayed alive with this module | [0, 1] | `RegisterSurvivalContribution(timeSurvived, totalBattleTime)` |
| `offensiveSynergy` | How much module enabled weapons/drones to perform | [0, 1] | `RegisterOffensiveSynergy(synergyScore)` |
| `powerEfficiency` | Power resources available vs consumed | [0, ?) | `RegisterPowerEfficiency(powerAvailable, powerConsumed)` |
| `roleFulfillment` | How well module matched its genome mapping | [0, 1] | `RegisterRoleFulfillment(mapping3DDistance)` |
| `battleDuration` | Total combat duration | [0, ?) | `RegisterBattleOutcome(duration, survived)` |
| `survivalSuccess` | Whether ship survived the encounter | {0, 1} | `RegisterBattleOutcome(duration, survived)` |

**Why These Are Better:**
- ? **Universal** - All metrics apply equally to all module types
- ? **Outcome-based** - Measure *results*, not assumptions
- ? **Gameplay-aligned** - Directly tied to what matters (survival, enabling weapons)
- ? **Independent** - Don't duplicate `statScore` from fitness calculation
- ? **Fair** - Tank modules, power modules, and drone modules can all score well

---

### 2. **Fitness.cs** - Updated Calculation

**Changed:**
```csharp
// OLD: stat-biased tracker score
float avoidNorm = Mathf.Clamp01(t.damageAvoided / 3000f);
float powerNorm = Mathf.Clamp01(t.powerSaved / 2000f);
float heatNorm = Mathf.Clamp01(t.heatReduced / 2000f);
float trackerScore = (avoidNorm * 0.5f) + (powerNorm * 0.3f) + (heatNorm * 0.2f);
```

**To:**
```csharp
// NEW: universal metrics that favor outcome over stat type
float efficiencyNorm = Mathf.Clamp01(t.damageEfficiency / 50f);
float survivalNorm = Mathf.Clamp01(t.survivalContribution);
float synergyNorm = Mathf.Clamp01(t.offensiveSynergy);
float roleFulfillmentNorm = Mathf.Clamp01(t.roleFulfillment);

float trackerScore = 
    (efficiencyNorm * 0.35f) +      // Damage prevention per stat
    (survivalNorm * 0.40f) +        // Overall survival contribution
    (synergyNorm * 0.15f) +         // Support for weapons
    (roleFulfillmentNorm * 0.10f);  // Role alignment
```

**Weighting Rationale:**
- **Survival (40%)** - Most important: did the module keep the ship alive?
- **Efficiency (35%)** - How well did stats translate to actual defense?
- **Synergy (15%)** - Did it enable weapons to do their job?
- **Role Alignment (10%)** - Did it match its intended specialization?

---

### 3. **EvolutionLog.cs** - Extended Tracking

**Added to ShipGenomeLog:**
- `damageEfficiency`
- `survivalContribution`
- `offensiveSynergy`
- `powerEfficiency`
- `roleFulfillment`
- `battleDuration`
- `survivalSuccess`

**Backward Compatibility:**
- Legacy metrics still logged: `timeEquipped`, `damageAvoided`, `powerSaved`, `heatReduced`
- JSON files can be analyzed with both old and new metrics

---

### 4. **New: BattleMetricsRecorder.cs**

A new system for recording metrics during gameplay:

```csharp
// At wave start
BattleMetricsRecorder.Instance.BeginBattle();

// When damage is mitigated
BattleMetricsRecorder.Instance.RegisterModuleDamageMitigation(moduleId, damageAmount);

// Periodically track survival
BattleMetricsRecorder.Instance.RegisterModuleSurvivalTime(moduleId, timeSurvived, totalTime);

// When weapons fire
BattleMetricsRecorder.Instance.RegisterModuleOffensiveSynergy(moduleId, synergyScore);

// Power management
BattleMetricsRecorder.Instance.RegisterModulePowerEfficiency(moduleId, available, consumed);

// Role fulfillment
BattleMetricsRecorder.Instance.RegisterModuleRoleFulfillment(moduleId, intended, actual);

// At wave end
ModuleManager.Instance.RegisterBattleMetricsForAll(duration, playerSurvived);
```

---

### 5. **CombatEventRouter.cs** - Extended Events

**New Methods:**
- `ReportModuleDamageMitigated(moduleId, damageAmount)` - Primary method for damage metrics
- `ReportDamageAvoided()` - Legacy compatibility wrapper

**Backward Compatibility:**
- Existing calls still work
- New metrics automatically calculated from reported data

---

### 6. **ModuleManager.cs** - Utility Methods

**New Public Methods:**
```csharp
// Get all modules for batch processing
List<(ShipGenome, ModuleStatsTracker)> GetAllModules();

// Register metrics for all modules at once
void RegisterBattleMetricsForAll(float duration, bool survived);

// Clear battle metrics for new wave
void ResetMetricsForAll();
```

---

## Integration Checklist

To fully integrate the new system into your gameplay:

- [ ] Update `WaveManager` to call `BattleMetricsRecorder.BeginBattle()` at wave start
- [ ] Update `WaveManager` to call `ModuleManager.RegisterBattleMetricsForAll()` at wave end
- [ ] Update `PlayerHealth.TakeDamage()` to call `ReportModuleDamageMitigated()` for each defensive module
- [ ] Update weapon controllers to calculate and report `offensiveSynergy` scores
- [ ] Update power management to report `powerEfficiency` metrics
- [ ] Implement `CalculateActualPerformanceMapping()` for role fulfillment tracking
- [ ] Test fitness scores improve with new metrics
- [ ] Verify JSON logs include new metrics

See **INTEGRATION_GUIDE.txt** for detailed code examples.

---

## Backward Compatibility

? **All existing code still compiles and runs.**

- Legacy metric methods marked `[Obsolete]` but still functional
- `CombatEventRouter.ReportDamageAvoided()` still works as a wrapper
- JSON logs include both old and new metrics
- Fitness calculation automatically uses new metrics

**Migration Path:**
1. Keep legacy code running as-is
2. Add new metric registration calls incrementally
3. Old metrics gradually become redundant as new data accumulates
4. Eventually remove legacy methods after validation period

---

## Example: Tank Module vs Power Module

**Before (Biased):**
- Tank module: High `damageAvoided` ? High trackerScore
- Power module: Low `damageAvoided` ? Low trackerScore
- **Result:** Tank modules always win, power modules never evolve

**After (Fair):**
- Tank module: High `damageEfficiency`, High `survivalContribution` ? High trackerScore ?
- Power module: High `offensiveSynergy`, High `survivalContribution` ? High trackerScore ?
- **Result:** Both can win if they keep the ship alive; fitness depends on *outcome*, not *stat type*

---

## Normalization Reference

When implementing metric registration in gameplay:

| Metric | Typical Range | Normalization Divisor |
|--------|---------------|----------------------|
| `damageEfficiency` | 0-100+ | 50f (adjust per difficulty) |
| `survivalContribution` | [0, 1] | Already normalized |
| `offensiveSynergy` | [0, 1] | Already normalized |
| `powerEfficiency` | 0.5-3.0 | 2.0f |
| `roleFulfillment` | [0, 1] | Already normalized |

---

## Files Modified

1. ? **ModuleStatsTracker.cs** - New metrics system
2. ? **Fitness.cs** - Updated IndividualFitness() for ShipGenome
3. ? **EvolutionLog.cs** - Extended ShipGenomeLog, updated CreateShipLogEntry()
4. ? **CombatEventRouter.cs** - New metric reporting methods
5. ? **ModuleManager.cs** - Utility methods for batch metric operations

## Files Created

1. ? **BattleMetricsRecorder.cs** - Central system for gameplay metric recording
2. ? **INTEGRATION_GUIDE.txt** - Implementation examples and best practices

---

## Testing Recommendations

1. **Unit Tests:**
   - Verify normalization of metrics to expected ranges
   - Test edge cases (division by zero, infinity)
   - Validate fitness calculation weights sum correctly

2. **Integration Tests:**
   - Run evolution with new metrics enabled
   - Compare fitness improvement vs old system
   - Verify diverse module types survive longer

3. **Data Analysis:**
   - Compare logs from old vs new metrics
   - Analyze correlation between metrics and survival
   - Verify fair distribution across module types

---

## Questions or Issues?

Refer to **INTEGRATION_GUIDE.txt** for detailed examples of how to implement each metric in your specific gameplay systems.
