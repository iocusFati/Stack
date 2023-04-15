using System.Collections.Generic;
using System.Linq;
using Infrastructure.AssetProviderService;
using Infrastructure.Data;
using UnityEngine;
using UnityEngine.Pool;

namespace Gameplay.BlockFolder.Pool
{
    public class BlockPool : IBlockPool
    {
        private readonly IAssets _assetProvider;

        private readonly Vector3 _initialScale;
        
        private readonly List<Block> _blockPoolList = new();

        private IObjectPool<Block> _blockPool;

        public List<Block> ActiveBlocks => _blockPoolList
            .Where(block => block.gameObject.activeSelf)
            .ToList();

        private IObjectPool<Block> Pool
        {
            get
            {
                return _blockPool ??= new ObjectPool<Block>(
                    SpawnBlock,
                    block => { block.gameObject.SetActive(true); }, 
                    block => { block.gameObject.SetActive(false); },
                    block => { Object.Destroy(block.gameObject); });
            }
        }

        public BlockPool(IAssets assetProvider, BlockStaticData blockData)
        {
            _assetProvider = assetProvider;

            _initialScale = blockData.StarterBlockScale;
        }

        public Block GetBlock()
        {
            Block block = Pool.Get();
            _blockPoolList.Add(block);
            block.transform.SetParent(null);

            SetScaleOneToOne(block);

            return block;
        }

        public void Release(Block block)
        {
            Pool.Release(block);
            _blockPoolList.Remove(block);
        }

        private Block SpawnBlock()
        {
            Block block = _assetProvider.Instantiate<Block>(AssetPaths.Block);
            Transform pivotAdjuster = new GameObject("Pivot").transform;
            
            block.Construct(this);
            block.Initialize();
            
            block.SetPivotAdjuster(pivotAdjuster);

            return block;
        }

        private void SetScaleOneToOne(Block block)
        {
            Transform blockTransform = block.transform;
            Transform blockMesh = block.Mesh;

            blockMesh.SetParent(blockTransform);
            blockMesh.localScale = Vector3.one;
            blockMesh.localPosition = Vector3.zero;
            blockMesh.localRotation = Quaternion.Euler(Vector3.zero);
            
            blockTransform.localScale = _initialScale;

            block.PivotAdjuster.localScale = Vector3.one;
        }
    }
}