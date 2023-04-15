using DG.Tweening;
using Infrastructure.Data;
using UnityEngine;
using Utils;

namespace Gameplay.BlockFolder
{
    public class BlockMovement : IBlockMovement
    {
        private const int FirstTweenId = 1;
        private const int SecondTweenId = 2;

        private readonly float _speedDelta;
        private readonly float _starterScaleZ;
        private readonly float _maxSpeed;
        private readonly float _initialSpeed;

        private Vector3 _diffInPos;
        private Block _block;

        public float Speed { get; private set; }

        public BlockMovement(BlockStaticData blockData)
        {
            _initialSpeed = Speed = blockData.Speed;
            _maxSpeed = blockData.MaxSpeed;
            _speedDelta = blockData.SpeedDelta;

            _starterScaleZ = blockData.StarterBlockScale.z;
        }

        public void StartShuffling(Block block, Vector3 spawnedAt)
        {
            Transform blockTransform = block.transform;
            Vector3 spawnedAtPreviousPosDiff = block.LastBlockPos - spawnedAt;
            
            float distance = spawnedAtPreviousPosDiff.MaxAxis() + (_starterScaleZ - blockTransform.lossyScale.z);
            float duration = distance / Speed;
            
            if (Speed + _speedDelta <= _maxSpeed) 
                Speed += _speedDelta;

            Shuffle(block, blockTransform, spawnedAtPreviousPosDiff.MaxAxis(), duration);
        }

        private void Shuffle(Block block, Transform blockTransform, float distance, float duration)
        {
            blockTransform
                .DOMove(
                    GetDestination(
                        block, blockTransform.forward, distance), duration)
                .SetEase(Ease.Flash)
                .SetId(FirstTweenId)
                .OnComplete(() => blockTransform
                    .DOMove(
                        GetDestination(
                            block, -blockTransform.forward, distance), duration)
                    .SetEase(Ease.Flash)
                    .SetId(SecondTweenId)
                    .OnComplete(() => Shuffle(block, blockTransform, distance, duration)));
        }

        public void StopShuffling()
        {
            DOTween.Kill(FirstTweenId);
            DOTween.Kill(SecondTweenId);
        }

        public void ResetMovementDuration() => 
            Speed = _initialSpeed;

        private Vector3 GetDestination(Block block, Vector3 direction, float spawnedAtPreviousPosDiff)
        {
            Transform blockTransform = block.transform;

            return new Vector3(direction.x * spawnedAtPreviousPosDiff, blockTransform.position.y,
                direction.z * spawnedAtPreviousPosDiff) + block.LastBlockPos.ExceptY();
        }
    }
}