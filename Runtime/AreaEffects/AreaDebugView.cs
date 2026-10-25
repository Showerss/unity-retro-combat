using System.Collections.Generic;
using UnityEngine;

namespace Retro.Combat
{
    /// <summary>
    /// Debug overlay for <see cref="AreaQuery"/>: briefly draws each query's sphere in the Game view,
    /// with a green line to every target it hit and a red line to every target a wall shielded.
    /// On by default in the Editor and development builds; flip <see cref="Enabled"/> to toggle.
    /// </summary>
    public sealed class AreaDebugView : MonoBehaviour
    {
        public static bool Enabled = Debug.isDebugBuild;
        public static float Duration = 1f;

        private const int RingSegments = 48;
        private const float LineWidth = 0.03f;
        private static readonly Color SphereColor = new Color(1f, 0.85f, 0.2f, 0.9f);
        private static readonly Color HitColor = new Color(0.2f, 1f, 0.3f, 1f);
        private static readonly Color BlockedColor = new Color(1f, 0.2f, 0.2f, 1f);

        private static Material _material;
        private float _endTime;

        public static void Show(Vector3 center, float radius, List<Vector3> hits, List<Vector3> blocked)
        {
            var root = new GameObject("AreaDebugView").AddComponent<AreaDebugView>();
            root.transform.position = center;
            root._endTime = Time.time + Duration;

            root.AddRing(center, radius, Vector3.right, Vector3.forward); // horizontal
            root.AddRing(center, radius, Vector3.right, Vector3.up);
            root.AddRing(center, radius, Vector3.forward, Vector3.up);
            foreach (Vector3 p in hits) root.AddLine(center, p, HitColor);
            foreach (Vector3 p in blocked) root.AddLine(center, p, BlockedColor);
        }

        private void Update()
        {
            if (Time.time >= _endTime) Destroy(gameObject);
        }

        private void AddRing(Vector3 center, float radius, Vector3 a, Vector3 b)
        {
            LineRenderer lr = CreateLine(SphereColor);
            lr.loop = true;
            lr.positionCount = RingSegments;
            for (int i = 0; i < RingSegments; i++)
            {
                float t = i * Mathf.PI * 2f / RingSegments;
                lr.SetPosition(i, center + (a * Mathf.Cos(t) + b * Mathf.Sin(t)) * radius);
            }
        }

        private void AddLine(Vector3 from, Vector3 to, Color color)
        {
            LineRenderer lr = CreateLine(color);
            lr.positionCount = 2;
            lr.SetPosition(0, from);
            lr.SetPosition(1, to);
        }

        private LineRenderer CreateLine(Color color)
        {
            var go = new GameObject("Line");
            go.transform.SetParent(transform, false);
            var lr = go.AddComponent<LineRenderer>();
            lr.useWorldSpace = true;
            lr.widthMultiplier = LineWidth;
            lr.startColor = lr.endColor = color;
            lr.sharedMaterial = GetMaterial();
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;
            return lr;
        }

        // Sprites/Default is unlit, vertex-coloured and works in both built-in and URP.
        private static Material GetMaterial()
        {
            if (_material == null) _material = new Material(Shader.Find("Sprites/Default")) { name = "AreaDebugView" };
            return _material;
        }
    }
}
