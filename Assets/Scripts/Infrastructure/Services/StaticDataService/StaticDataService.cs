using Infrastructure.AssetProviderService;
using Infrastructure.Data;
using Infrastructure.Data.CameraData;
using Infrastructure.Data.UIData;
using UnityEngine;

namespace Infrastructure.Services.StaticDataService
{
    public class StaticDataService : IStaticDataService
    {
        public BlockStaticData BlockData { get; private set; }
        public CameraStaticData CameraData { get; private set; }
        public ColorStaticData ColorData { get; private set; }
        public UIStaticData UIData { get; private set; }

        public void Initialize()
        {
            LoadBlockData();
            LoadCameraData();
            LoadColorData();
            LoadUIData();
        }

        private void LoadBlockData() => 
            BlockData = Resources.Load<BlockStaticData>(AssetPaths.BlockData);

        private void LoadCameraData() => 
            CameraData = Resources.Load<CameraStaticData>(AssetPaths.CameraData);

        private void LoadColorData() => 
            ColorData = Resources.Load<ColorStaticData>(AssetPaths.ColorData);
        private void LoadUIData() => 
            UIData = Resources.Load<UIStaticData>(AssetPaths.UIData);
    }
}