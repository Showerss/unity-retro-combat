using UnityEngine;
using UnityEngine.Events;

namespace Retro.Combat
{
    /// <summary>
    /// Hit points for any object. Fires events so visuals and game logic stay outside this class.
    /// </summary>
    public sealed class Health : MonoBehaviour, IDamageable
    {
        [SerializeField, Min(1f)] private float _maxHealth = 100f;

        [Tooltip("Destroy this GameObject when health reaches zero.")]
        [SerializeField] private bool _destroyOnDeath;

        [Tooltip("Log each hit to the console. Handy on test dummies.")]
        [SerializeField] private bool _logDamage;

        public UnityEvent<float> OnDamaged = new UnityEvent<float>();
        public UnityEvent OnDied = new UnityEvent();

        public float Current { get; private set; }
        public float Max => _maxHealth;
        public bool IsDead => Current <= 0f;

        private void Awake() => Current = _maxHealth;

        public void TakeDamage(DamageInfo info)
        {
            if (IsDead || info.Amount <= 0f) return;

            Current = Mathf.Max(0f, Current - info.Amount);
            if (_logDamage) Debug.Log($"{name} took {info.Amount:0.#} damage ({Current:0.#}/{_maxHealth} HP)", this);
            OnDamaged.Invoke(info.Amount);

            if (!IsDead) return;
            OnDied.Invoke();
            if (_destroyOnDeath) Destroy(gameObject);
        }

        public void ResetHealth() => Current = _maxHealth;
    }
}
