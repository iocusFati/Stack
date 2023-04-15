using Infrastructure.Services;
using UnityEngine;

namespace Gameplay.BlockFolder
{
    public interface IBlockMovement : IService
    {
        float Speed { get; }
        void StartShuffling(Block block, Vector3 SpawnedAt);
        void StopShuffling();
        void ResetMovementDuration();
    }
}