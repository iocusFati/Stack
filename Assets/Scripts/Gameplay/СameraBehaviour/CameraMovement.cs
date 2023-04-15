using System;
using DG.Tweening;
using Gameplay.BlockFolder;
using Infrastructure.Data.CameraData;
using Infrastructure.Factories;
using UnityEngine;
using Utils;

namespace Gameplay.СameraBehaviour
{
    public class CameraMovement : ICameraMovement
    {
        private readonly IBlockFactory _blockFactory;
        
        private readonly float _minHeight;
        private readonly float _moveYDuration;
        private readonly float _gameLostRotAngleX;
        private readonly float _loseRotationDuration;

        private Camera _camera;
        private Transform _latestBlock;
        private Transform _cameraTransform;
        private Transform _camHolder;

        private float _firstDot;
        private float _initialPosY;

        public CameraMovement(IBlockFactory blockFactory, CameraStaticData cameraStaticData)
        {
            _blockFactory = blockFactory;
            
            _minHeight = cameraStaticData.MinHeight;
            _moveYDuration = cameraStaticData.MoveYDuration;
            _gameLostRotAngleX = cameraStaticData.GameLostRotAngleX;
            _loseRotationDuration = cameraStaticData.LoseRotationDuration;
            
            blockFactory.OnFirstBlockCreated += SetFirstBlock; 
            blockFactory.OnBlockCreated += RaiseCamera;
        }

        private void SetFirstBlock(Block block)
        {
            _blockFactory.OnFirstBlockCreated -= SetFirstBlock;
            _initialPosY = _cameraTransform.position.y;
        }

        public void SetMainCamera()
        {
            _camera = Camera.main;
            _cameraTransform = _camera.transform;
            _camHolder = _cameraTransform.parent;
        }

        public void LoseRotate(Action onRotated, bool rotateBack = false)
        {
            Vector3 rotation = _cameraTransform.localRotation.eulerAngles;
            _cameraTransform
                .DOLocalRotate(new Vector3(
                    rotation.x + -MyMath.BoolToInt(rotateBack) * _gameLostRotAngleX, rotation.y, rotation.z)
                    , _loseRotationDuration)
                .OnComplete(onRotated.Invoke);
        }

        public void PosYToInitial()
        {
            Vector3 position = _camHolder.position;
            
            position = new Vector3(position.x, _initialPosY, position.z);
            _camHolder.position = position;
        }

        private void RaiseCamera(Block block)
        {
            float camBlockHeightDiff = _cameraTransform.position.y - block.transform.position.y;

            if (camBlockHeightDiff < _minHeight)
                _camHolder.DOMoveY(block.transform.position.y + _minHeight, _moveYDuration);
        }

    }
}