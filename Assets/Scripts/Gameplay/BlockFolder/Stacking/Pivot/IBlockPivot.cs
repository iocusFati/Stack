using Utils;

namespace Gameplay.BlockFolder
{
    public interface IBlockPivot
    {
        void SetPivotPos(Block block, NumSign isNegative, Axis axis);
    }
}