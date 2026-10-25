# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- `Poison` status effect + `PoisonSettings`: damage over time that lingers after exposure; re-exposure refreshes duration and keeps the stronger dose.
- `DamageZone._poison`: poisons everything caught inside.
- `AreaDebugView`: shows each area query's radius, hit targets and LOS-blocked targets for a second.

## [0.1.0] - 2026-10-24

### Added
- `IDamageable`, `DamageInfo`, `Health`.
- `Explosion` (radius damage with falloff + knockback) and `DamageZone` (lingering damage over time), sharing `AreaQuery`.
- `FuseTimer` and `ImpactTrigger` for thrown objects.
