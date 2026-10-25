# Retro Combat

Health/damage and area effects for thrown or placed objects. No dependencies.

```
IDamageable / DamageInfo   contract for anything that can be hurt
└── Health                 HP + OnDamaged/OnDied events

Area effects (share AreaQuery: overlap sphere, deduped, optional line of sight)
├── Explosion              one-shot falloff damage + knockback + light flash, call Detonate()
└── DamageZone             damage every tick for a duration, then despawns

Triggers (wire through UnityEvents, mix freely)
├── FuseTimer              OnFuseEnd after N seconds
└── ImpactTrigger          OnImpact on first hard collision, optional spawn prefab
```

## Recipes
- **Bomb:** Rigidbody + collider + `FuseTimer` (OnFuseEnd → `Explosion.Detonate`) + `Explosion`.
- **Impact grenade:** swap `FuseTimer` for `ImpactTrigger` (OnImpact → `Explosion.Detonate`, `_destroyOnImpact` off).
- **Beehive / poison flask:** `ImpactTrigger` with `_spawnOnImpact` = a prefab holding `DamageZone`.
- **Target:** add `Health` to any object with a collider.

See `Documentation~/Combat.md` for design notes.
