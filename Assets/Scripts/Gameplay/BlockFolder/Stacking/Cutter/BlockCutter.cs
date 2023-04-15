using Infrastructure.Factories;
using UnityEngine;
using Utils;

namespace Gameplay.BlockFolder.Cutter
{
    public class BlockCutter : IBlockCutter
    {
        private readonly IBlockPivot _pivot;
        private readonly IBlockFactory _blockFactory;

        public BlockCutter(IBlockPivot pivot, IBlockFactory blockFactory)
        {
            _pivot = pivot;
            _blockFactory = blockFactory;
        }

        public void CutOffExtra(Block block, Vector3 diffInPos)
        {
            Transform blockTransform = block.transform;
            
            Axis scaleIn = default;
            float pivotScaleZ = default;
            NumSign isNegative = default;

            Vector3 pivotLocalScale = block.PivotAdjuster.localScale;
            float maxComponent = Mathf.Max(Mathf.Abs(diffInPos.x), Mathf.Abs(diffInPos.z));

            if (Mathf.Abs(diffInPos.x) == maxComponent)
            {
                isNegative = MyMath.NumSign(diffInPos.x);
                _pivot.SetPivotPos(block, MyMath.NegateSign(isNegative), Axis.Z);

                scaleIn = Axis.X;
                pivotScaleZ = MyMath.FindProportion(
                    Mathf.Abs(diffInPos.x), blockTransform.lossyScale.z, pivotLocalScale.x);
            }            
            else  if (Mathf.Abs(diffInPos.z) == maxComponent)
            {
                isNegative = MyMath.NumSign(diffInPos.z); 
                _pivot.SetPivotPos(block, isNegative, Axis.Z);

                scaleIn = Axis.Z;
                pivotScaleZ = MyMath.FindProportion(
                    diffInPos.z * -(int)isNegative, blockTransform.lossyScale.z, pivotLocalScale.z);
            }

            Vector3 pivotScale = new Vector3(0, 0, pivotScaleZ);
            
            block.PivotAdjuster.localScale = pivotLocalScale - pivotScale;
            CreateStump(block, diffInPos, isNegative, scaleIn);
        }

        private void CreateStump(Block block, Vector3 diffInPos, NumSign isNegative, Axis scaleIn)
        {
            Vector3 cutOffScale = ScaleStump(block, diffInPos, scaleIn);

            _blockFactory.CreateBlockStump(block, cutOffScale, MyMath.SignToBool(isNegative));
        }

        private Vector3 ScaleStump(Block block, Vector3 diffInPos, Axis scaleIn)
        {
            Vector3 cutOffScale = default;
            Vector3 blockScale = block.transform.lossyScale;

            cutOffScale = scaleIn switch
            {
                Axis.X => new Vector3(blockScale.x, blockScale.y, Mathf.Abs(diffInPos.x)),
                Axis.Z => new Vector3(blockScale.x, blockScale.y, Mathf.Abs(diffInPos.z)),
                _ => cutOffScale
            };

            return cutOffScale;
        }
    }
}