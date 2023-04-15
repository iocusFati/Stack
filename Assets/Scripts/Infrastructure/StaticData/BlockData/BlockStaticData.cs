using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Infrastructure.Data
{
    [CreateAssetMenu(fileName = "BlockData", menuName = "StaticData/BlockData")]
    public class BlockStaticData : ScriptableObject
    {
        [Header("Spawning")]
        public float FirstBlockScaleDuration;
        [FormerlySerializedAs("SpawnOffset")] public float BaseSpawnOffset;
        public float OffsetSpeedModifier;

        public bool FirstSpawnAtFront;
        
        public Vector3 StarterBlockScale;
        public Vector3 StarterBlockRot;

        [Header("Movement")]
        public float Speed;
        public float MaxSpeed;
        public float SpeedDelta;

        [Header("Stacking")]
        public int TimesSnappingBeforeScalingUp;

        public float AcceptableAccuracyError;
        public float MinStackScale;
        public float ScaleDuration;
        public float AddScale;


        [Header("StuckParticle")]
        public float ParticleRadius;
        public float OffsetFromBlock;
        
        public int MaxTimesParticlesChange;
        
        public List<Vector3> RotationsFromFront;
        public List<Vector3> RotationsFromSide;
    }
}