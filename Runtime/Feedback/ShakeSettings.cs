using System;
using UnityEngine;

namespace Retro.Combat
{
    /// <summary>Inspector-friendly <see cref="ScreenShake.Pulse"/> parameters. Amplitude 0 = no shake.</summary>
    [Serializable]
    public struct ShakeSettings
    {
        [Tooltip("Peak camera offset in metres at the source. Keep it to a few centimetres.")]
        [Min(0f)] public float Amplitude;

        [Tooltip("Distance (m) at which the shake fades to nothing.")]
        [Min(0f)] public float Radius;

        [Tooltip("Seconds for the shake to die out.")]
        [Min(0.01f)] public float Duration;

        public ShakeSettings(float amplitude, float radius, float duration)
        {
            Amplitude = amplitude;
            Radius = radius;
            Duration = duration;
        }

        public void Play(Vector3 source) => ScreenShake.Pulse(source, Amplitude, Radius, Duration);
    }
}
