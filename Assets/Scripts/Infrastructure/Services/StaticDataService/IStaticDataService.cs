using Infrastructure.Data;
using Infrastructure.Data.CameraData;
using Infrastructure.Data.UIData;

namespace Infrastructure.Services.StaticDataService
{
    public interface IStaticDataService : IService
    {
        BlockStaticData BlockData { get; }
        CameraStaticData CameraData { get; }
        ColorStaticData ColorData { get; }
        UIStaticData UIData { get; }
    }
}