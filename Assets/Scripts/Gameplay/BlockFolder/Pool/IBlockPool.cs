using System.Collections.Generic;
using Infrastructure.Services;

namespace Gameplay.BlockFolder.Pool
{
    public interface IBlockPool : IService
    {
        List<Block> ActiveBlocks { get; }
        Block GetBlock();
        void Release(Block block);
    }
}