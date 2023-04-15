using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.Data
{
    [CreateAssetMenu(fileName = "ColorData", menuName = "StaticData/ColorData")]
    public class ColorStaticData : ScriptableObject
    {
        [Header("Palettes")]
        public List<BlockBackgroundColors> ColorPalette;
        public TwoColorPack StackParticlesPalette;

        [Header("Configurable")]
        public int ColorsBetweenLastAndFirst;
        public int NumOfColorsInBetween_Block;
        public int NumOfColorsInBetween_Skybox;
        
        public float GradientDuration;

        [Header("StackParticle")] 
        [Range(0, 1)]
        public float ColorOverTimeAlpha;
        public int BaseEmissionRateOverTime;
        public int AddToEmissionRate;
    }
}