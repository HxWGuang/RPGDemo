using System;
using UnityEngine;

namespace RPG.Core
{
    public class DestroyEffect : MonoBehaviour
    {
        [SerializeField] private GameObject targetToDestroy = null;
        
        private ParticleSystem _effectObj = null;

        private void Awake()
        {
            _effectObj = GetComponent<ParticleSystem>();
        }

        void Update()
        {
            if (!_effectObj.IsAlive())
            {
                Destroy(targetToDestroy != null ? targetToDestroy : gameObject);
            }
        }
    }
}
