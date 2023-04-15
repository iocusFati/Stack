using Infrastructure.AssetProviderService;
using UnityEngine;
using UnityEngine.Pool;

namespace Gameplay.BlockFolder.Pool
{
    public class StumpPool
    {
        private const string StumpTag = "Stump";
        private readonly IAssets _assetProvider;
        
        private IObjectPool<Transform> _stumpPool;
        // private readonly List<Transform> _activeCoins = new();

        private IObjectPool<Transform> Pool
        {
            get
            {
                return _stumpPool ??= new ObjectPool<Transform>(
                    SpawnStump,
                    stump => { stump.gameObject.SetActive(true); }, 
                    stump => { stump.gameObject.SetActive(false); },
                    stump => { Object.Destroy(stump.gameObject); });
            }
        }

        public StumpPool(IAssets assetProvider) => 
            _assetProvider = assetProvider;

        public Transform GetBlock()
        {
            Transform stump = Pool.Get();
            
            return stump;
        }

        public void Release(Transform stump)
        {
            if (stump.CompareTag(StumpTag)) 
                Pool.Release(stump);
        }

        private Transform SpawnStump() => 
            _assetProvider.Instantiate<Transform>(AssetPaths.Stump);
    } 
}
