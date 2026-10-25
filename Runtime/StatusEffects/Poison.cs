using System;
using UnityEngine;

namespace Retro.Combat
{
    /// <summary>
    /// How hard and how long a poison bites. Duration 0 = no poison.
    /// </summary>
    [Serializable]
    public struct PoisonSettings
    {
        [Min(0f)] public float DamagePerTick;
        [Min(0.05f)] public float TickInterval;
        [Tooltip("Seconds the poison lasts after the last exposure. 0 = no poison.")]
        [Min(0f)] public float Duration;

        public bool IsActive => Duration > 0f && DamagePerTick > 0f;

        public PoisonSettings(float damagePerTick, float tickInterval, float duration)
        {
            DamagePerTick = damagePerTick;
            TickInterval = tickInterval;
            Duration = duration;
        }
    }

    /// <summary>
    /// Damage over time that stays on a target after it leaves the source. Re-exposure refreshes it, it doesn't stack.
    /// Added at runtime by <see cref="Apply"/>; removes itself when it wears off.
    /// </summary>
    public sealed class Poison : MonoBehaviour
    {
        private IDamageable _target;
        private GameObject _source;
        private float _damagePerTick;
        private float _tickInterval;
        private float _nextTick;
        private float _endTime;

        public bool IsPoisoned => Time.time < _endTime;

        public static void Apply(IDamageable target, PoisonSettings settings, GameObject source)
        {
            if (!settings.IsActive || !(target is Component host)) return;

            Poison poison = host.GetComponent<Poison>();
            if (poison == null)
            {
                poison = host.gameObject.AddComponent<Poison>();
                poison._target = target;
                poison._nextTick = Time.time + settings.TickInterval; // the source already bit this frame
            }

            poison._source = source;
            poison._damagePerTick = Mathf.Max(poison.IsPoisoned ? poison._damagePerTick : 0f, settings.DamagePerTick);
            poison._tickInterval = settings.TickInterval;
            poison._endTime = Mathf.Max(poison._endTime, Time.time + settings.Duration);
        }

        private void Update()
        {
            if (!IsPoisoned)
            {
                Destroy(this);
                return;
            }

            if (Time.time < _nextTick) return;
            _nextTick += _tickInterval;
            _target.TakeDamage(new DamageInfo(_damagePerTick, transform.position, _source));
        }
    }
}
