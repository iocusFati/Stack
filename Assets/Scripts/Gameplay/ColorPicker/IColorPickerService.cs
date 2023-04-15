using Infrastructure.Services;
using UnityEngine;

namespace Gameplay.ColorPicker
{
    public interface IColorPickerService : IService
    {
        void SelectPalette();   
        Color PickBlockColor();
        (Color, Color) PickSkyboxColors();
        (Color, Color) PickStackParticleColors(int numOfStack);
    }
}