using System.Collections.Generic;
using UnityEngine;

namespace Retro.Combat
{
    /// <summary>
    /// Finds damageables and rigidbodies inside a sphere, once each, optionally only those in line of sight.
    /// </summary>
    public static class AreaQuery
    {
        private static readonly Collider[] Hits = new Collider[64];
        private static readonly HashSet<Object> Seen = new HashSet<Object>();

        public static void Collect(Vector3 center, float radius, LayerMask mask, LayerMask occluders, bool requireLineOfSight,
            List<(IDamageable target, Vector3 point)> damageables, List<Rigidbody> bodies)
        {
            damageables?.Clear();
            bodies?.Clear();
            Seen.Clear();

            int count = Physics.OverlapSphereNonAlloc(center, radius, Hits, mask, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < count; i++)
            {
                Collider col = Hits[i];
                IDamageable target = damageables != null ? col.GetComponentInParent<IDamageable>() : null;
                Rigidbody rb = bodies != null ? col.attachedRigidbody : null;
                if (rb != null && rb.isKinematic) rb = null;
                if (target == null && rb == null) continue; // terrain, walls

                // LOS aims at the collider's center so rays from a bomb on the floor don't skim the ground.
                if (requireLineOfSight && IsBlocked(center, col.bounds.center, col, occluders)) continue;

                if (target is Object key && Seen.Add(key)) damageables.Add((target, ClosestPoint(col, center)));
                if (rb != null && Seen.Add(rb)) bodies.Add(rb);
            }
        }

        // Physics.ClosestPoint only supports primitives and convex meshes.
        private static Vector3 ClosestPoint(Collider col, Vector3 point)
        {
            bool supported = col is BoxCollider || col is SphereCollider || col is CapsuleCollider
                             || (col is MeshCollider mesh && mesh.convex);
            return supported ? col.ClosestPoint(point) : col.bounds.ClosestPoint(point);
        }

        private static bool IsBlocked(Vector3 from, Vector3 to, Collider target, LayerMask occluders)
        {
            Vector3 dir = to - from;
            float dist = dir.magnitude;
            if (dist < 0.01f) return false;

            return Physics.Raycast(from, dir / dist, out RaycastHit hit, dist, occluders, QueryTriggerInteraction.Ignore)
                   && hit.collider != target
                   && hit.collider.attachedRigidbody == null; // loose props don't shield
        }
    }
}
