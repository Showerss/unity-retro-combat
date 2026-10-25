using UnityEngine;

namespace Retro.Combat
{
    /// <summary>
    /// Self-destroying point light that fades out. Used instead of camera shake for impact feedback.
    /// </summary>
    public sealed class LightFlash : MonoBehaviour
    {
        private Light _light;
        private float _startIntensity;
        private float _duration;
        private float _elapsed;

        public static LightFlash Spawn(Vector3 position, Color color, float intensity, float range, float duration)
        {
            var go = new GameObject("LightFlash");
            go.transform.position = position;
            var light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.shadows = LightShadows.None;

            var flash = go.AddComponent<LightFlash>();
            flash._light = light;
            flash._startIntensity = intensity;
            flash._duration = Mathf.Max(0.01f, duration);
            return flash;
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            float t = _elapsed / _duration;
            if (t >= 1f)
            {
                Destroy(gameObject);
                return;
            }
            _light.intensity = _startIntensity * (1f - t);
        }
    }
}
