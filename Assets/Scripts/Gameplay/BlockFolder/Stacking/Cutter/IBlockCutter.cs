using UnityEngine;

namespace Gameplay.BlockFolder.Cutter
{
    public interface IBlockCutter
    {
        void CutOffExtra(Block block, Vector3 diffInPos);
    }
}