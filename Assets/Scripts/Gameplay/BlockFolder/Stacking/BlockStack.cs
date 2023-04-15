using Gameplay.BlockFolder.Cutter;
using Gameplay.BlockFolder.Pool;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Factories;
using Infrastructure.Services.Input;
using Infrastructure.States;
using UI.Entities;
using UI.Factory;
using UnityEngine;
using Utils;

namespace Gameplay.BlockFolder
{
    public class BlockStack : ITickable
    {
        private IGameStateMachine _gameStateMachine;
        private GameLostState _scoreContainer;
        private IBlockMovement _blockMovement;
        private IBlockFactory _blockFactory;
        private IInputService _inputService;
        private BlockParticle _blockParticle;

        private float _acceptableAccuracyError;
        private float _minStackScale;
        private int _timesSnappingBeforeScalingUp;
        private int _timesSnappedInARow;

        private Block _block;
        private Transform _blockTransform;
        private IBlockCutter _blockCutter;
        private IBlockScaler _blockScaler;
        private IBlockPivot _pivot;
        private IUIEntitiesGetter _UIEntities;
        private IBlockPool _blockPool;

        public void Construct(
            ITicker ticker,
            IBlockPool blockPool,
            IGameStateMachine gameStateMachine,
            IInputService inputService,
            IBlockFactory blockFactory,
            IUIEntitiesGetter entities,
            IBlockMovement blockMovement,
            BlockParticle blockParticle,
            BlockStaticData blockData)
        {
            _blockPool = blockPool;
            _gameStateMachine = gameStateMachine;
            _inputService = inputService;
            _UIEntities = entities;
            _blockFactory = blockFactory;
            _blockMovement = blockMovement;
            _blockParticle = blockParticle;
            
            _acceptableAccuracyError = blockData.AcceptableAccuracyError;
            _minStackScale = blockData.MinStackScale; 
            _timesSnappingBeforeScalingUp = blockData.TimesSnappingBeforeScalingUp;
            
            ticker.AddTickable(this);
            blockFactory.OnBlockCreated += SetBlock;
            
            _pivot = new BlockPivot();
            _blockCutter = new BlockCutter(_pivot, _blockFactory);
            _blockScaler = new BlockScaler(blockFactory, _pivot, blockData);
        }

        public void Tick()
        {
            if (_inputService.Tap() && _gameStateMachine.IsInGameLoop)
            {
                _blockMovement.StopShuffling();
                StackBlock();
            }
        }
        
        private void SetBlock(Block block)
        {
            _block = block;
            _blockTransform = block.transform;
        }

        private void StackBlock()
        {
            Vector3 diffInPos = _block.LastBlockPos.ExceptY() - _blockTransform.position.ExceptY();
            float stackScalePercentage = _blockTransform.lossyScale.z * _acceptableAccuracyError;
            float stackScale = 
                _minStackScale > stackScalePercentage
                ? _minStackScale
                : stackScalePercentage;
            
            ReleaseInvisible();

            if (DiffIsTooBig(diffInPos))
            {
                Rigidbody blockRB = _block.gameObject.AddComponent<Rigidbody>();
                Object.Destroy(blockRB, 2);
                
                _gameStateMachine.Enter<GameLostState>();
            }            
            else
            {
                if (diffInPos.magnitude < stackScale)
                {
                    SnapBlocks();
                    _blockParticle.TimesStacked = ++_timesSnappedInARow;

                    if (_timesSnappedInARow >= _timesSnappingBeforeScalingUp)
                        _blockScaler.AdjustScaleZ(_block);
                }
                else
                    CutOff(diffInPos);

                RaiseScore();
                _blockFactory.CreateBlock();
            }
        }

        private void ReleaseInvisible()
        {
            for (int i = 1; i < _blockPool.ActiveBlocks.Count; i++)
            {
                if (_blockPool.ActiveBlocks[i].CheckIfVisibleToCamera()) continue;

                _blockPool.ActiveBlocks[i].Release();
                i--;
            }
        }

        private void SnapBlocks()
        {
            Vector3 lastBlockPos = _block.LastBlockPos;
            _blockTransform.position = new Vector3(lastBlockPos.x, _blockTransform.position.y, lastBlockPos.z);
            
            _blockParticle.Snap(_block);
        }

        private void RaiseScore() => 
            _UIEntities.Single<ScoreContainer>().RaiseScore();

        private bool DiffIsTooBig(Vector3 diffInPos)
        {
            return diffInPos.magnitude > _blockTransform.lossyScale.z;
        }

        private void CutOff(Vector3 diffInPos)
        {
            _blockCutter.CutOffExtra(_block, diffInPos);
            _blockParticle.TimesStacked = _timesSnappedInARow = 0;
        }
    }
}
