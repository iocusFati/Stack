using System;
using DG.Tweening;
using Gameplay.BlockFolder;
using Gameplay.BlockFolder.Pool;
using Gameplay.ColorPicker;
using Infrastructure.AssetProviderService;
using Infrastructure.Data;
using Infrastructure.States;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

namespace Infrastructure.Factories
{
    public class BlockFactory : IBlockFactory
    {
        private readonly IBlockMovement _blockMovement;
        private readonly IBlockPool _blockPool;
        private readonly GameLostState _scoreContainer;
        private readonly IColorPickerService _colorPicker;
        private readonly IAssets _assets;

        private readonly Vector3 _starterBlockScale;
        private readonly Vector3 _starterBlockRot;
        private readonly float _firstBlockScaleDuration;
        private readonly float _baseSpawnOffset;
        private readonly float _offsetSpeedModifier;

        private bool _floatFromFront;
        private Transform _lastBlock;
        private ParticleSystem _stackParticle;
        private BlockParticle _blockParticle;

        public event Action<Block> OnBlockCreated;
        public event Action<Block> OnFirstBlockCreated;

        public BlockFactory(
            IAssets assets,
            IBlockPool blockPool,
            IColorPickerService colorPicker,
            IBlockMovement blockMovement,
            BlockStaticData blockData)
        {
            _assets = assets;
            _blockPool = blockPool;
            _colorPicker = colorPicker;
            _blockMovement = blockMovement;

            _baseSpawnOffset = blockData.BaseSpawnOffset;
            _offsetSpeedModifier = blockData.OffsetSpeedModifier;
            _firstBlockScaleDuration = blockData.FirstBlockScaleDuration;
            _floatFromFront = blockData.FirstSpawnAtFront;
            _starterBlockScale = blockData.StarterBlockScale;
            _starterBlockRot = blockData.StarterBlockRot;
        }

        public void CreateFirstBlock(bool replayCreation = false, Action onCreated = null)
        {
            var block = _blockPool.GetBlock();
            var blockTransform = block.transform;
            
            blockTransform.position = Vector3.zero;
            SetFirstBlockScale(block, replayCreation, NotifyFirstBlockCreated);
            blockTransform.localRotation = Quaternion.Euler(_starterBlockRot);

            _lastBlock = blockTransform;

            _colorPicker.SelectPalette();
            block.Material.color = _colorPicker.PickBlockColor();

            _blockMovement.ResetMovementDuration();

            void NotifyFirstBlockCreated()
            {
                OnFirstBlockCreated.Invoke(block);
                onCreated?.Invoke();
            }
        }

        public Block CreateBlock(bool moveOnCreate)
        {
            var block = _blockPool.GetBlock();
            var blockTransform = block.transform;
            var lastBlockPos = _lastBlock.position;
            var lastBlockScale = _lastBlock.lossyScale;
            
            block.SetPreviousBlockData(lastBlockPos);
            block.FloatFromFront = _floatFromFront;
            
            blockTransform.localScale = new Vector3(lastBlockScale.z, lastBlockScale.y, lastBlockScale.x);
            var spawnedAt = PlaceBlock(blockTransform);
            blockTransform.LookAtAxis(lastBlockPos, Vector3.up);
            block.PivotAdjuster.rotation = blockTransform.rotation;

            AlignPivotWithBlock(block);

            block.Material.color = _colorPicker.PickBlockColor();

            _floatFromFront  = !_floatFromFront;
            _lastBlock = blockTransform;
            
            if (moveOnCreate)
                _blockMovement.StartShuffling(block, spawnedAt);
            
            OnBlockCreated.Invoke(block);
            
            return block;
        }

        public void CreateBlockStump(Block origin, Vector3 scale, bool diffInPosIsNegative)
        {
            Transform stump = _assets.Instantiate<Transform>(AssetPaths.Stump);
            Transform originTransform = origin.transform;
            
            stump.rotation = originTransform.rotation;
            stump.localScale = scale;
            stump.GetComponent<MeshRenderer>().material.color = origin.Material.color;

            Vector3 stumpPos = CalculateStumpPosition(origin, diffInPosIsNegative, stump);
            stump.position = stumpPos;
            
            Object.Destroy(stump.gameObject, 5);
        }
        
        private Vector3 CalculateStumpPosition(Block origin, bool diffInPosIsNegative, Transform stump)
        {
            Transform originTransform = origin.transform;
            Vector3 originPos = originTransform.position;
            Vector3 pivotAdjusterPos = origin.PivotAdjuster.position;
            float scaleZ = originTransform.lossyScale.z;

            Vector3 stumpPos;
            if (!origin.FloatFromFront)
                stumpPos = CalculateStumpPositionForZAxis(
                    pivotAdjusterPos, scaleZ, stump.localScale.z, diffInPosIsNegative, originPos.y, originPos.z);
            else
                stumpPos = CalculateStumpPositionForXAxis(
                    pivotAdjusterPos, scaleZ, stump.localScale.z, diffInPosIsNegative, originPos.x, originPos.y);

            return stumpPos;
        }

        private Vector3 CalculateStumpPositionForZAxis(Vector3 pivotAdjusterPos, float scaleZ, 
            float stumpScaleZ, bool diffInPosIsNegative, float posY, float posZ)
        {
            float offsetX = diffInPosIsNegative 
                ? scaleZ + stumpScaleZ * 0.5f 
                : -scaleZ - stumpScaleZ * 0.5f;
            Vector3 stumpPos = new Vector3(pivotAdjusterPos.x + offsetX, posY, posZ);
            
            return stumpPos;
        }

        private Vector3 CalculateStumpPositionForXAxis(Vector3 pivotAdjusterPos, float scaleZ, 
            float stumpScaleZ, bool diffInPosIsNegative, float posX, float posY)
        {
            float offsetZ = diffInPosIsNegative 
                ? scaleZ + stumpScaleZ * 0.5f 
                : -scaleZ - stumpScaleZ * 0.5f;
            Vector3 stumpPos = new Vector3(posX, posY, pivotAdjusterPos.z + offsetZ);
            
            return stumpPos;
        }

        private void AlignPivotWithBlock(Block block)
        {
            Transform blockTransform = block.transform;
            Transform blockPivotAdjuster = block.PivotAdjuster;
            
            blockPivotAdjuster.RotWithoutAltChild(blockTransform.rotation);
            blockPivotAdjuster.position = blockTransform.position;
        }

        private Vector3 PlaceBlock(Transform blockTransform)
        {
            Vector3 lastBlockPos = _lastBlock.transform.position;

            float scaleZ = blockTransform.lossyScale.z > 1 ? blockTransform.lossyScale.z : 1;
            float offset = _baseSpawnOffset * scaleZ * _blockMovement.Speed * _offsetSpeedModifier;

            if (_floatFromFront)
                blockTransform.position = new Vector3(
                    lastBlockPos.x, PlaceY(), lastBlockPos.z + offset);
            else
                blockTransform.position = new Vector3(
                    lastBlockPos.x - offset, PlaceY(), lastBlockPos.z);

            float PlaceY() => 
                _lastBlock.position.y + _lastBlock.lossyScale.y * 0.5f + blockTransform.localScale.y * 0.5f;

            return blockTransform.position;
        }
        
        private void SetFirstBlockScale(Block block, bool replayCreation, Action onCreated)
        {
            block.transform.localScale = _starterBlockScale;

            if (replayCreation)
            {
                block.Mesh.localScale = Vector3.zero;
                block.Mesh
                    .DOScale(Vector3.one, _firstBlockScaleDuration)
                    .OnComplete(onCreated.Invoke);
            }
            else
                onCreated.Invoke();
        }
    }
}
