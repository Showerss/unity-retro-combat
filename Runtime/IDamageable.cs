using UnityEngine;

namespace Retro.Combat
{
    /// <summary>
    /// Anything that can be hurt: enemies, dummies, breakable props.
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(DamageInfo info);
    }

    /// <summary>
    /// One hit: how much, where, and who caused it.
    /// </summary>
    public readonly struct DamageInfo
    {
        public readonly float Amount;
        public readonly Vector3 Point;
        public readonly GameObject Source;

        public DamageInfo(float amount, Vector3 point, GameObject source)
        {
            Amount = amount;
            Point = point;
            Source = source;
        }
    }
}
