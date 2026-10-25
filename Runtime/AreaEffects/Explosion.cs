using System.Collections.Generic;
using UnityEngine;

namespace Retro.Combat
{
    /// <summary>
    /// One-shot blast: damage with distance falloff plus knockback, then the object is removed.
    /// Call <see cref="Detonate"/> from a trigger (FuseTimer, ImpactTrigger) or code.
    /// </summary>
    public sealed class Explosion : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float _radius = 4f;
        [SerializeField, Min(0f)] private float _damage = 60f;

        [Tooltip("Fraction of damage dealt at the edge of the radius.")]
        [Range(0f, 1f)]
        [SerializeField] private float _edgeDamage = 0.25f;

        [Tooltip("Knockback speed (m/s) at the center, fading to zero at the radius. Mass-independent.")]
        [SerializeField, Min(0f)] private float _knockback = 8f;
        [SerializeField, Min(0f)] private float _upwardsModifier = 1f;

        [SerializeField] private LayerMask _affects = ~0;

        [Tooltip("Walls and terrain that block the blast.")]
        [SerializeField] private LayerMask _occluders = ~0;
        [SerializeField] private bool _requireLineOfSight = true;

        [Header("Feedback")]
        [Tooltip("Spawned at the blast point (flash, smoke). Destroy itself.")]
        [SerializeField] private GameObject _vfxPrefab;
        [SerializeField] private AudioClip _sound;
        [Range(0f, 1f)]
        [SerializeField] private float _volume = 1f;

        [Tooltip("Brief point light flash. 0 disables it.")]
        [SerializeField, Min(0f)] private float _flashIntensity = 8f;
        [SerializeField] private Color _flashColor = new Color(1f, 0.7f, 0.35f);

        [Tooltip("Remove this GameObject after detonating.")]
        [SerializeField] private bool _destroyAfter = true;

        private readonly List<(IDamageable target, Vector3 point)> _targets = new List<(IDamageable, Vector3)>();
        private readonly List<Rigidbody> _bodies = new List<Rigidbody>();
        private bool _detonated;

        public float Radius => _radius;

        public void Detonate()
        {
            if (_detonated) return;
            _detonated = true;

            Vector3 center = transform.position;
            Rigidbody self = GetComponent<Rigidbody>();
            if (self != null) self.detectCollisions = false; // don't shield or shove ourselves

            AreaQuery.Collect(center, _radius, _affects, _occluders, _requireLineOfSight, _targets, _bodies);

            foreach (var (target, point) in _targets)
            {
                float t = Vector3.Distance(center, point) / _radius;
                float amount = _damage * Mathf.Lerp(1f, _edgeDamage, t);
                target.TakeDamage(new DamageInfo(amount, point, gameObject));
            }

            foreach (Rigidbody rb in _bodies)
            {
                if (rb != self) rb.AddExplosionForce(_knockback, center, _radius, _upwardsModifier, ForceMode.VelocityChange);
            }

            PlayFeedback(center);
            if (_destroyAfter) Destroy(gameObject);
        }

        private void PlayFeedback(Vector3 center)
        {
            if (_vfxPrefab != null) Instantiate(_vfxPrefab, center, Quaternion.identity);
            if (_sound != null) AudioSource.PlayClipAtPoint(_sound, center, _volume);
            if (_flashIntensity > 0f) LightFlash.Spawn(center, _flashColor, _flashIntensity, _radius * 2f, 0.25f);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.4f, 0f, 0.35f);
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
