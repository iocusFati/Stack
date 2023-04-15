using UnityEngine;

namespace Infrastructure.Data.CameraData
{
    [CreateAssetMenu(fileName = "CameraData", menuName = "StaticData/CameraData")]
    public class CameraStaticData : ScriptableObject
    {
        [Header("Moving")]
        public float MinHeight;
        public float MoveYDuration;

        [Header("Lose")]
        public float TimeBeforeLose;
        public float GameLostRotAngleX;
        public float LoseRotationDuration;
    }
}