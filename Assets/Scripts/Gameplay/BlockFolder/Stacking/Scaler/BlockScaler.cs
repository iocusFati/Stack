using DG.Tweening;
using Infrastructure.Data;
using Infrastructure.Factories;
using UnityEngine;
using Utils;

namespace Gameplay.BlockFolder
{
    public class BlockScaler : IBlockScaler
    {
        private readonly IBlockPivot _pivot;
        private readonly Vector3 _starterScale;
        private readonly float _scaleDuration;
        private readonly float _addScale;

        private Block _firstBlock;
        
        private Transform _mainFrontPoint;
        private Transform _mainRightPoint;
        private Transform _mainLeftPoint;

        public BlockScaler(
            IBlockFactory blockFactory, 
            IBlockPivot pivot, 
            BlockStaticData blockData)
        {
            _pivot = pivot;
            _starterScale = blockData.StarterBlockScale;
            _scaleDuration = blockData.ScaleDuration;
            _addScale = blockData.AddScale;

            blockFactory.OnFirstBlockCreated += block =>
            {
                Debug.Log("OnFirstBlockCreated");
                _mainFrontPoint = block.FrontPoint;
                _mainRightPoint = block.RightPoint;
                _mainLeftPoint = block.LeftPoint;
            };
        }

        public void AdjustScaleZ(Block block)
        {
            Transform blockTransform = block.transform;
            float blockScaleZ = blockTransform.lossyScale.z;
            
            float spaceForward = Mathf.Abs(FindFrontDiff(block));
            float spaceBack = Mathf.Abs(_starterScale.z - blockScaleZ - spaceForward);
            
            // Debug.Log("Space Back: " + spaceBack);
            // Debug.Log("Space forward: " + spaceForward);

            block.Mesh.SetParent(null);
            blockTransform.SetParent(null);

            if (blockScaleZ < _starterScale.z)
            {
                if (spaceForward >= spaceBack)
                {
                    _pivot.SetPivotPos(block, NumSign.Negative, Axis.Z);
                
                    float scaler = DecideScaler(spaceForward);
                    ScaleForward(block, scaler);
                }
                else if (spaceForward < spaceBack)
                {
                    _pivot.SetPivotPos(block, NumSign.Positive, Axis.Z);

                    float scaler = DecideScaler(spaceBack);
                    ScaleForward(block, scaler);
                }
            }
            else
                AdjustScaleX(block);
        }
        public void AdjustScaleX(Block block)
        {
            Transform blockTransform = block.transform;
            float blockScaleX = blockTransform.lossyScale.x;
            int blockFloatsFromFront = MyMath.BoolToInt(block.FloatFromFront);

            float spaceForward = Mathf.Abs(FindSideDiff(block));
            float spaceBack = Mathf.Abs(_starterScale.x - blockScaleX - spaceForward);
            
            // Debug.Log("Space Back: " + spaceBack);
            // Debug.Log("Space forward: " + spaceForward);

            block.Mesh.SetParent(null);
            // blockTransform.SetParent(null);
            
            if (blockScaleX < _starterScale.z)
            {
                if (spaceForward >= spaceBack)
                {
                    var numSign = MyMath.NumSign((int)NumSign.Negative * blockFloatsFromFront);
                    _pivot.SetPivotPos(block, numSign, Axis.X);

                    float scaler = DecideScaler(spaceForward);
                    ScaleSide(block, scaler);
                }
                else if (spaceForward < spaceBack)
                {
                    var numSign = MyMath.NumSign((int)NumSign.Positive * blockFloatsFromFront);
                    _pivot.SetPivotPos(block, numSign, Axis.X);

                    float scaler = DecideScaler(spaceBack);
                    ScaleSide(block, scaler);
                }
            }
        }
        
        
        private void ScaleForward(Block block, float addScale)
        {
            Transform blockTransform = block.transform;
            Vector3 blockScale = blockTransform.lossyScale;
            Vector3 initialPivotScale = block.PivotAdjuster.localScale;
            
            addScale = MyMath.FindProportion(
                addScale, blockScale.z, block.PivotAdjuster.lossyScale.z);

            block.PivotAdjuster.localScale += Vector3.forward * addScale;
            blockTransform.SetParent(null);
            
            block.PivotAdjuster.localScale = initialPivotScale;
            block.Mesh.SetParent(block.PivotAdjuster);

            block.PivotAdjuster.DOScaleZ(
                block.PivotAdjuster.localScale.z + (Vector3.forward * addScale).z, _scaleDuration);
        }

        private void ScaleSide(Block block, float addScale)
        {
            Transform blockTransform = block.transform;
            Vector3 blockScale = blockTransform.lossyScale;
            
            addScale = MyMath.FindProportion(
                addScale, blockScale.x, block.PivotAdjuster.lossyScale.x);
            
            block.PivotAdjuster.localScale += Vector3.right * addScale;
            block.Mesh.SetParent(block.PivotAdjuster);

            block.PivotAdjuster.DOScaleX(
                block.PivotAdjuster.localScale.x + (Vector3.right * addScale).x, _scaleDuration);
        }

        private float FindFrontDiff(Block block)
        {
            float diffInPos;

            if (block.FloatFromFront)
                diffInPos = _mainFrontPoint.position.z - block.FrontPoint.position.z;
            else
                diffInPos = _mainRightPoint.position.x - block.FrontPoint.position.x;

            return diffInPos;
        }

        private float FindSideDiff(Block block)
        {
            float diffInPos;

            if (block.FloatFromFront)
                diffInPos = _mainRightPoint.position.x - block.RightPoint.position.x;
            else
                diffInPos = _mainFrontPoint.position.z - block.LeftPoint.position.z;

            return diffInPos;
        }

        private float DecideScaler(float spaceForward)
        {
            if (spaceForward < _addScale)
                return spaceForward;
            else if (spaceForward > _addScale)
                return _addScale;
            
            return default;
        }

    }
}