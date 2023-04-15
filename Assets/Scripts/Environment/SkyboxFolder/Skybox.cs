using System.Collections;
using Gameplay.BlockFolder;
using Gameplay.ColorPicker;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Factories;
using UnityEngine;

namespace DefaultNamespace.Environment.SkyboxFolder
{
    public class Skybox : MonoBehaviour
    {
        [SerializeField] private Material _skybox;
        
        private static readonly int Color1 = Shader.PropertyToID("_Color1");
        private static readonly int Color2 = Shader.PropertyToID("_Color2");

        private IColorPickerService _colorPicker;
        private ICoroutineRunner _coroutineRunner;

        private float _gradientDuration;
        private bool _isChangingColor;

        public void Construct(
            IColorPickerService colorPicker, 
            IBlockFactory blockFactory, 
            ICoroutineRunner coroutineRunner,
            ColorStaticData colorData)
        {
            _colorPicker = colorPicker;
            _coroutineRunner = coroutineRunner;

            _gradientDuration = colorData.GradientDuration;

            blockFactory.OnBlockCreated += SwitchSkyboxColor;
        }

        private void SwitchSkyboxColor(Block block)
        {
            if (_isChangingColor) return;
            
            _isChangingColor = true;
            _coroutineRunner.StartCoroutine(GradientSkybox());
        }

        private IEnumerator GradientSkybox()
        {
            float time = 0;
            float lerpInterpolant = 0;
            float interpolantDelta =  1 / (1 * _gradientDuration / Time.deltaTime);
            
            (Color, Color) colors = _colorPicker.PickSkyboxColors();
            Color initialColor1 = _skybox.GetColor(Color1);
            Color initialColor2 = _skybox.GetColor(Color2);

            while (time < _gradientDuration)
            {
                var color1 = Color.Lerp(initialColor1, colors.Item1, lerpInterpolant);
                var color2 = Color.Lerp(initialColor2, colors.Item2, lerpInterpolant);

                _skybox.SetColor(Color1, color1);
                _skybox.SetColor(Color2, color2);

                time += Time.deltaTime;
                lerpInterpolant += interpolantDelta;

                yield return null;
            }

            _isChangingColor = false;
        }
    }
}