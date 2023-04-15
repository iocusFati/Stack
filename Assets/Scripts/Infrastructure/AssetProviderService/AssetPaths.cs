using Infrastructure.Data;
using Infrastructure.Data.CameraData;

namespace Infrastructure.AssetProviderService
{
    public abstract class AssetPaths
    {
        public const string BlockData = "Data/BlockData";
        public const string CameraData = "Data/CameraData";
        public const string ColorData = "Data/ColorData";
        public const string UIData = "Data/UIData";

        public const string StackParticle = "Prefabs/Particles/StackParticle";
        public const string Block = "Prefabs/Block/Block";

        public const string Skybox = "Prefabs/Skybox";
        public const string Stump = "Prefabs/Stump";

        public const string UIRoot = "Prefabs/UI/UIRoot";
        public const string ScoreContainer = "Prefabs/UI/Windows/ScoreContainer";
        public const string LoseWindow = "Prefabs/UI/Windows/LoseWindow";
        public const string MainMenu = "Prefabs/UI/Windows/MainMenuWindow";
    }
}