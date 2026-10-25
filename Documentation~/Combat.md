# Combat: design notes

- **Damage goes through `IDamageable` only.** Area effects never know about `Health`, so enemies can implement their own reactions.
- **One hit per target per query.** `AreaQuery` dedupes by the `IDamageable` component (found with `GetComponentInParent`), so multi-collider enemies are not hit several times.
- **Line of sight** raycasts from the center to each collider's bounds center (distance falloff still uses the closest point). Loose rigidbodies (crates) don't block, but static geometry does. Turn it off per effect with `_requireLineOfSight`.
- **Explosion falloff** is linear from `_damage` at the center to `_damage * _edgeDamage` at the radius.
- **No camera shake.** Feedback is a light flash + sound, for motion comfort. If shake is ever added, it must be opt-in and off by default.
- **Poison refreshes, never stacks.** Standing in a zone resets the timer to its full duration and keeps the higher damage per tick, so damage stays predictable. `Poison` is added to the `IDamageable`'s GameObject and removes itself when it wears off. Its first tick waits one interval because the zone already bit that frame.
- Triggers are decoupled from effects through UnityEvents, so designers can wire any combination in a prefab without code.
