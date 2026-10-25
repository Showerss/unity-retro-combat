using UnityEngine;
using UnityEngine.Events;

namespace Retro.Combat
{
    /// <summary>
    /// Fires <see cref="OnImpact"/> once, the first time this rigidbody hits something hard enough.
    /// Spawns an optional prefab at the hit point (e.g. a bee swarm) and can remove itself.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class ImpactTrigger : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _minRelativeSpeed = 1.5f;

        [Tooltip("Ignore hits for this long after spawning, so it can leave the thrower's hand.")]
        [SerializeField, Min(0f)] private float _armDelay = 0.05f;

        [Tooltip("Spawned at the impact point, upright.")]
        [SerializeField] private GameObject _spawnOnImpact;
        [SerializeField] private AudioClip _impactSound;
        [SerializeField] private bool _destroyOnImpact = true;

        public UnityEvent OnImpact = new UnityEvent();

        private float _armedAt;
        private bool _fired;

        private void OnEnable()
        {
            _fired = false;
            _armedAt = Time.time + _armDelay;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_fired || Time.time < _armedAt) return;
            if (collision.relativeVelocity.magnitude < _minRelativeSpeed) return;
            _fired = true;

            Vector3 point = collision.contactCount > 0 ? collision.GetContact(0).point : transform.position;
            if (_spawnOnImpact != null) Instantiate(_spawnOnImpact, point, Quaternion.identity);
            if (_impactSound != null) AudioSource.PlayClipAtPoint(_impactSound, point);

            OnImpact.Invoke();
            if (_destroyOnImpact) Destroy(gameObject);
        }
    }
}
