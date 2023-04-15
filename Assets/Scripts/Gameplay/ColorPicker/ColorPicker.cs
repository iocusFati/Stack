using System.Collections.Generic;
using Infrastructure.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.ColorPicker
{
    public class ColorPicker : IColorPickerService
    {
        private readonly int _numOfColorsInBetween;
        private readonly int _colorsBetweenLastAndFirst;
        private readonly TwoColorPack _stackParticlesPalettes;
        private readonly BlockBackgroundColors[] _colorPalettesVariants;

        private BlockBackgroundColors _stackColors;
        private Color _lastColor;
        private Queue<Color> _blockPalette;
        private Queue<Color> _skyboxPalette1;
        private Queue<Color> _skyboxPalette2;

        public ColorPicker(ColorStaticData colorData)
        {
            _numOfColorsInBetween = colorData.NumOfColorsInBetween_Block;
            _colorPalettesVariants = colorData.ColorPalette.ToArray();
            _colorsBetweenLastAndFirst = colorData.ColorsBetweenLastAndFirst;
            _stackParticlesPalettes = colorData.StackParticlesPalette;
        }

        public void SelectPalette()
        {
            _stackColors = _colorPalettesVariants[Random.Range(0, _colorPalettesVariants.Length)];
            _blockPalette = new Queue<Color>();

            _skyboxPalette1 = new Queue<Color>(_stackColors.BackgroundColor.ColorPaletteOne); 
            _skyboxPalette2 = new Queue<Color>(_stackColors.BackgroundColor.ColorPaletteTwo); 
            
            FulfillPalette(_stackColors.BlockPalette, _numOfColorsInBetween, 
                _colorsBetweenLastAndFirst, _blockPalette);
            FulfillPalette(
                _stackColors.BackgroundColor.ColorPaletteOne, _numOfColorsInBetween, 
                _colorsBetweenLastAndFirst, _skyboxPalette1);
            FulfillPalette(
                _stackColors.BackgroundColor.ColorPaletteTwo, _numOfColorsInBetween, 
                _colorsBetweenLastAndFirst, _skyboxPalette2);
        }

        private void FulfillPalette(List<Color> blockPalette, int numOfColorsInBetween, int colorsBetweenLastAndFirst,
            Queue<Color> palette)
        {
            for (int i = 0; i < blockPalette.Count - 1; i++)
                SetColorsBetween(blockPalette[i], blockPalette[i + 1], numOfColorsInBetween, palette);

            SetColorsBetween(blockPalette[^1], blockPalette[0], colorsBetweenLastAndFirst, palette);
        }

        public Color PickBlockColor()
        {
            Color result = _blockPalette.Dequeue();
            _blockPalette.Enqueue(result);

            return result;
        }

        public (Color, Color) PickSkyboxColors()
        {
            (Color, Color) result = (_skyboxPalette1.Dequeue(), _skyboxPalette2.Dequeue());
            _skyboxPalette1.Enqueue(result.Item1);
            _skyboxPalette2.Enqueue(result.Item2);
            
            return result;
        }

        public (Color, Color) PickStackParticleColors(int numOfStack) =>
            (_stackParticlesPalettes.ColorPaletteOne[numOfStack],
                _stackParticlesPalettes.ColorPaletteTwo[numOfStack]);

        private void SetColorsBetween(Color color1, Color color2, int colorsInBetween, Queue<Color> palette)
        {
            float lerpInterpolant = 0;
            for (int i = 0; i < colorsInBetween; i++)
            {
                palette.Enqueue(
                    Color.Lerp(
                        a: color1, 
                        b: color2,
                        t: lerpInterpolant += (float)1 / colorsInBetween));
            }
        }
    }
}