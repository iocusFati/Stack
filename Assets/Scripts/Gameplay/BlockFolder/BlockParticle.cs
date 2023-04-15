using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using Gameplay.BlockFolder.Particle;
using Gameplay.ColorPicker;
using Infrastructure.AssetProviderService;
using Infrastructure.Data;
using UnityEngine;
using Utils;

namespace Gameplay.BlockFolder
{
    public class BlockParticle
    {
        private readonly IAssets _assets;
        private readonly IColorPickerService _colorPicker;

        private ParticleSystem _headStackParticle;

        private readonly float _particleRadius;
        private readonly float _offsetFromBlock;
        private readonly float _colorOverTimeAlpha;
        private readonly int _baseEmissionRateOverTime;
        private readonly int _addToEmissionRate;
        private readonly int _maxTimesStacked;

        private ParticlePool _stackParticlePool;

        private int _timesStacked;
        
        private readonly List<Vector3> _rotationsFromFront;
        private readonly List<Vector3> _rotationsFromSide;

        public int TimesStacked
        {
            get => 
                _timesStacked;
            set => 
                _timesStacked = value <= _maxTimesStacked - 1 
                    ? value 
                    : _maxTimesStacked;
        }

        public BlockParticle(
            IColorPickerService colorPicker,
            IAssets assets,
            ColorStaticData colorData,
            BlockStaticData blockData)
        {
            _colorPicker = colorPicker;
            _assets = assets;

            _colorOverTimeAlpha = colorData.ColorOverTimeAlpha;
            _baseEmissionRateOverTime = colorData.BaseEmissionRateOverTime;
            _addToEmissionRate = colorData.AddToEmissionRate;
            _particleRadius = blockData.ParticleRadius;
            _offsetFromBlock = blockData.OffsetFromBlock;
            _maxTimesStacked = blockData.MaxTimesParticlesChange;
            _rotationsFromFront = blockData.RotationsFromFront;
            _rotationsFromSide = blockData.RotationsFromSide;
        }

        public void Initialize()
        {
            _stackParticlePool = new ParticlePool(_assets);
            _stackParticlePool.Initialize(AssetPaths.StackParticle);
        }

        public ParticleSystem GetStackParticle() => 
            _stackParticlePool.GetParticle();

        public void Snap(Block block)
        {
            Transform blockTransform = block.transform;
            (Color color1, Color color2) = _colorPicker.PickStackParticleColors(TimesStacked);

            var colorOverLifetimeGradient = new Gradient();
            colorOverLifetimeGradient.SetKeys(
                new GradientColorKey[] { new(color1, 0), new(color2, 1) },
                new GradientAlphaKey[] { new(1, 0), new(_colorOverTimeAlpha, 0.6f) }
            );

            ParticleSystem headParticle = _stackParticlePool.GetParticle();
            ParticleSystem[] subStackParticles = headParticle
                .GetComponentsInChildren<ParticleSystem>(false)
                .Skip(1)
                .ToArray();

            var rotations = block.FloatFromFront ? _rotationsFromFront : _rotationsFromSide;
            var zAngle = 0;
            
            SetSubParticleTransform(subStackParticles[0], blockTransform,
                new Vector3(_particleRadius, blockTransform.lossyScale.z * 0.5f, 1), 
                new Vector3(MyMath.BoolToInt(block.FloatFromFront) *_offsetFromBlock, 0.5f, 0),
                rotations[0]);
            SetSubParticleTransform(subStackParticles[1], blockTransform,
                new Vector3(_particleRadius, blockTransform.lossyScale.z * 0.5f, 1), 
                new Vector3( MyMath.BoolToInt(block.FloatFromFront) * -_offsetFromBlock, 0.5f, 0),
                rotations[1]);
            SetSubParticleTransform(subStackParticles[2], blockTransform,
                new Vector3(blockTransform.lossyScale.x * 0.5f, _particleRadius, 1), 
                new Vector3(0, 0.5f, MyMath.BoolToInt(block.FloatFromFront) * _offsetFromBlock),
                rotations[2]);
            SetSubParticleTransform(subStackParticles[3], blockTransform,
                new Vector3(blockTransform.lossyScale.x * 0.5f, _particleRadius, 1), 
                new Vector3(0, 0.5f, MyMath.BoolToInt(block.FloatFromFront) * -_offsetFromBlock),
                rotations[3]);

            foreach (var particle in subStackParticles)
            {
                var main = particle.main;
                var colorOverLifetime = particle.colorOverLifetime;
                var emission = particle.emission;

                main.startColor = new ParticleSystem.MinMaxGradient(color1, color2);
                colorOverLifetime.color = new ParticleSystem.MinMaxGradient(colorOverLifetimeGradient);
                emission.rateOverTime = _baseEmissionRateOverTime + TimesStacked * _addToEmissionRate;
            }
            
            headParticle.Play();
        }

        private static void SetChildrenRot(ParticleSystem[] subStackParticles, Vector3 rot)
        {
            foreach (var particle in subStackParticles)
                particle.transform.localRotation = Quaternion.Euler(rot);
        }

        private void SetSubParticleTransform(
            ParticleSystem particle, 
            Transform block,
            Vector3 scale,
            Vector3 relativeToBlockPos,
            Vector3 rot)
        {
            var shape = particle.shape;
            shape.scale = scale;
            particle.transform.localRotation = Quaternion.Euler(rot);
            particle.transform.position = block.TransformPoint(relativeToBlockPos);
        }
    }
}