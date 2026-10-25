using System.Collections.Generic;
using UnityEngine;

namespace Retro.Combat
{
    /// <summary>
    /// Tiny positional camera jolt for impacts. Put it on the camera; call <see cref="Pulse"/> from anywhere.
    /// Comfort first: position only (never rotation), centimetre amplitudes, short decay, and a global
    /// <see cref="Strength"/> multiplier for an options slider (0 turns it off).
    /// </summary>
    public sealed class ScreenShake : MonoBehaviour
    {
        /// <summary>Global multiplier, 0..1. Wire this to a player setting.</summary>
        public static float Strength = 1f;

        private static readonly List<ScreenShake> Active = new List<ScreenShake>();

        [Tooltip("Hard cap on the offset in metres, however many pulses stack.")]
        [SerializeField, Min(0f)] private float _maxOffset = 0.05f;

        [Tooltip("Noise speed. Higher = buzzier, lower = a slower wobble.")]
        [SerializeField, Min(1f)] private float _frequency = 25f;

        private float _amplitude;
        private float _decayPerSecond;
        private Vector3 _offset;
        private Vector3 _applied;
        private float _seed;

        /// <summary>Shakes every listening camera, scaled down linearly to nothing at <c>radius</c>.</summary>
        public static void Pulse(Vector3 source, float amplitude, float radius, float duration)
        {
            if (amplitude <= 0f || Strength <= 0f) return;
            foreach (ScreenShake shake in Active)
            {
                float falloff = radius > 0f ? 1f - Vector3.Distance(source, shake.transform.position) / radius : 1f;
                if (falloff > 0f) shake.Add(amplitude * falloff * Strength, duration);
            }
        }

        private void Awake() => _seed = Random.value * 100f;

        private void OnEnable() => Active.Add(this);

        private void OnDisable()
        {
            Active.Remove(this);
            _amplitude = 0f;
            transform.localPosition = RemoveOffset(transform.localPosition);
            _offset = Vector3.zero;
        }

        private void Add(float amplitude, float duration)
        {
            // Keep the stronger of the current and new shake rather than summing, so bursts never pile up.
            if (amplitude <= _amplitude) return;
            _amplitude = Mathf.Min(amplitude, _maxOffset);
            _decayPerSecond = _amplitude / Mathf.Max(0.01f, duration);
        }

        // Runs after the controller's Update has placed the camera, so the jolt sits on top of head bob.
        private void LateUpdate()
        {
            Vector3 basePos = RemoveOffset(transform.localPosition);

            if (_amplitude > 0f)
            {
                _amplitude = Mathf.Max(0f, _amplitude - _decayPerSecond * Time.deltaTime);
                float t = Time.time * _frequency;
                _offset = new Vector3(Mathf.PerlinNoise(_seed, t) - 0.5f, Mathf.PerlinNoise(t, _seed) - 0.5f, 0f) * (2f * _amplitude);
            }
            else
            {
                _offset = Vector3.zero;
            }

            _applied = basePos + _offset;
            transform.localPosition = _applied;
        }

        // Only take our offset back off axes nobody else wrote this frame; an axis the controller overwrote is already clean.
        private Vector3 RemoveOffset(Vector3 p)
        {
            if (p.x == _applied.x) p.x -= _offset.x;
            if (p.y == _applied.y) p.y -= _offset.y;
            if (p.z == _applied.z) p.z -= _offset.z;
            return p;
        }
    }
}
