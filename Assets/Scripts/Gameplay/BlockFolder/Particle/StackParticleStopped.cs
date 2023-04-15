using System;
using UnityEngine;

namespace Gameplay.BlockFolder.Particle
{
    public class StackParticleStopped : MonoBehaviour
    {
        private ParticlePool _particlePool;
        private ParticleSystem _particle;

        public void Construct(ParticlePool particlePool)
        {
            _particlePool = particlePool;
            _particle = GetComponent<ParticleSystem>();
        }

        private void OnParticleSystemStopped()
        {
            _particlePool.Release(_particle);
        }
    }
}