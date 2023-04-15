using UnityEngine;
using Utils;

namespace Gameplay.BlockFolder
{
    public class BlockPivot : IBlockPivot
    {
        public void SetPivotPos(Block block, NumSign isNegative, Axis axis)
        {
            Transform blockTransform = block.transform;
            
            block.PivotAdjuster.position = axis switch
            {
                Axis.X => blockTransform.TransformPoint(0.5f * (int)isNegative, 0, 0),
                Axis.Z => blockTransform.TransformPoint(0, 0, 0.5f * (int)isNegative),
                _ => block.PivotAdjuster.position
            };
            
            blockTransform.SetParent(block.PivotAdjuster);
        }
    }
}