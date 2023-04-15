using System;
using Gameplay.BlockFolder;
using Infrastructure.Services;
using UnityEngine;

namespace Infrastructure.Factories
{
    public interface IBlockFactory : IService
    {
        event Action<Block> OnBlockCreated;
        event Action<Block> OnFirstBlockCreated;
        
        void CreateFirstBlock(bool replayCreation = false, Action onCreated = null);
        void CreateBlockStump(Block origin, Vector3 scale, bool diffInPosIsNegative);
        Block CreateBlock(bool moveOnCreate = true);
    }
}