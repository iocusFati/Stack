using Infrastructure.AssetProviderService;
using UnityEngine;
using UnityEngine.Pool;

namespace Gameplay.BlockFolder.Particle
{
    public class ParticlePool
    {
        private readonly IAssets _assetProvider;
        
        private IObjectPool<ParticleSystem> _blockPool;

        private string _pathToParticle;
        // private readonly List<Transform> _activeCoins = new();

        private IObjectPool<ParticleSystem> Pool
        {
            get
            {
                return _blockPool ??= new ObjectPool<ParticleSystem>(
                    SpawnParticle,
                    block => { block.gameObject.SetActive(true); }, 
                    block => { block.gameObject.SetActive(false); },
                    block => { Object.Destroy(block.gameObject); });
            }
        }

        public ParticlePool(IAssets assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public void Initialize(string pathToParticle)
        {
            _pathToParticle = pathToParticle;
        }

        public ParticleSystem GetParticle()
        {
            var particle = Pool.Get();
            return particle;
        }

        public void Release(ParticleSystem particle) => 
            Pool.Release(particle);

        private ParticleSystem SpawnParticle()
        {
            var particle = _assetProvider.Instantiate<ParticleSystem>(_pathToParticle);
            particle.GetComponent<StackParticleStopped>().Construct(this);
            
            return particle;
        }
    }
}