using UnityEngine;
using UnityEngine.Events;

namespace Retro.Combat
{
    /// <summary>
    /// Fires <see cref="OnFuseEnd"/> a set time after it is enabled (e.g. a lit bomb).
    /// </summary>
    public sealed class FuseTimer : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _seconds = 2f;

        [Tooltip("Optional looping hiss while the fuse burns.")]
        [SerializeField] private AudioSource _fuseLoop;

        public UnityEvent OnFuseEnd = new UnityEvent();

        private float _endTime;
        private bool _fired;

        private void OnEnable()
        {
            _fired = false;
            _endTime = Time.time + _seconds;
            if (_fuseLoop != null) _fuseLoop.Play();
        }

        private void Update()
        {
            if (_fired || Time.time < _endTime) return;
            _fired = true;
            if (_fuseLoop != null) _fuseLoop.Stop();
            OnFuseEnd.Invoke();
        }
    }
}
