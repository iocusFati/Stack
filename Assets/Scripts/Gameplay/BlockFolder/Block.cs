using Gameplay.BlockFolder.Pool;
using UnityEngine;

namespace Gameplay.BlockFolder
{
    public class Block : MonoBehaviour
    {
        public Transform FrontPoint;
        public Transform RightPoint;
        public Transform LeftPoint;
        public Transform Mesh;

        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Collider _collider;
        
        private Camera _camera;
        private BlockPool _blockPool;

        public Material Material { get; private set; }
        public Transform PivotAdjuster { get; private set; }
        public Vector3 LastBlockPos { get; private set; }
        public bool FloatFromFront { get; set; }

        public void Construct(BlockPool blockPool)
        {
            _blockPool = blockPool;
        }
        
        public void Initialize()
        {
            Material = _meshRenderer.material;
            _camera = Camera.main;
        }

        public void SetPreviousBlockData(Vector3 lastBlockPos)
        {
            LastBlockPos = lastBlockPos;
        }

        public void SetPivotAdjuster(Transform pivotAdjuster)
        {
            PivotAdjuster = pivotAdjuster;
        }

        public bool CheckIfVisibleToCamera()
        {
            Bounds bounds = _collider.bounds;
            Plane[] cameraFrustum = GeometryUtility.CalculateFrustumPlanes(_camera);

            return GeometryUtility.TestPlanesAABB(cameraFrustum, bounds);
        }

        public void Release()
        {
            Mesh.SetParent(transform);
            _blockPool.Release(this);
        }
    }
}