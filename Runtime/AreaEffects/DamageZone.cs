using System.Collections.Generic;
using UnityEngine;

namespace Retro.Combat
{
    /// <summary>
    /// Lingering area (bee swarm, poison cloud, fire): damages everything inside every tick, then despawns.
    /// </summary>
    public sealed class DamageZone : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float _radius = 2.5f;
        [SerializeField, Min(0f)] private float _damagePerTick = 5f;
        [SerializeField, Min(0.05f)] private float _tickInterval = 0.5f;

        [Tooltip("Seconds before the zone despawns. 0 = forever.")]
        [SerializeField, Min(0f)] private float _duration = 6f;

        [Tooltip("Poison applied to everything caught inside; keeps hurting after they leave.")]
        [SerializeField] private PoisonSettings _poison = new PoisonSettings(0f, 1f, 0f);

        [SerializeField] private LayerMask _affects = ~0;
        [SerializeField] private LayerMask _occluders = ~0;
        [SerializeField] private bool _requireLineOfSight = true;

        private readonly List<(IDamageable target, Vector3 point)> _targets = new List<(IDamageable, Vector3)>();
        private float _nextTick;
        private float _endTime;

        public float Radius => _radius;

        private void OnEnable()
        {
            _nextTick = Time.time; // first bite lands immediately
            _endTime = _duration > 0f ? Time.time + _duration : float.PositiveInfinity;
        }

        private void Update()
        {
            if (Time.time >= _endTime)
            {
                Destroy(gameObject);
                return;
            }

            if (Time.time < _nextTick) return;
            _nextTick += _tickInterval;

            AreaQuery.Collect(transform.position, _radius, _affects, _occluders, _requireLineOfSight, _targets, null);
            foreach (var (target, point) in _targets)
            {
                target.TakeDamage(new DamageInfo(_damagePerTick, point, gameObject));
                Poison.Apply(target, _poison, gameObject);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.85f, 0f, 0.35f);
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
