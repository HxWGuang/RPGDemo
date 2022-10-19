using System;
using GameDevTV.Saving;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematic
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        private bool _isPlayed = false;
        private PlayableDirector _playableDirector;

        private void Awake()
        {
            _playableDirector = GetComponent<PlayableDirector>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isPlayed && other.CompareTag("Player"))
            {
                _playableDirector.Play();
                _isPlayed = true;
            }
        }

        public object CaptureState()
        {
            return _isPlayed;
        }

        public void RestoreState(object state)
        {
            _isPlayed = (bool) state;
        }
    }
}
