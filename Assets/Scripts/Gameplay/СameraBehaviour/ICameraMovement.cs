using System;
using Infrastructure.Services;

namespace Gameplay.СameraBehaviour
{
    public interface ICameraMovement : IService
    {
        void SetMainCamera();
        void LoseRotate(Action onRotated, bool rotateBack = false);
        void PosYToInitial();
    }
}