using Gameplay.BlockFolder;
using Gameplay.BlockFolder.Pool;
using Gameplay.ColorPicker;
using Gameplay.СameraBehaviour;
using Infrastructure.AssetProviderService;
using Infrastructure.Factories;
using Infrastructure.Services;
using Infrastructure.Services.Input;
using Infrastructure.Services.PersistentProgress;
using Infrastructure.Services.SaveLoad;
using Infrastructure.Services.StaticDataService;
using UI.Factory;
using UnityEngine;

namespace Infrastructure.States
{
    public class BootstrapState : IState
    {
        private const string InitialSceneName = "Initial";
        private const string MainSceneName = "GameScene";
        
        private readonly IGameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly AllServices _services;
        private readonly UIHolder _UIEntities;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly ITicker _ticker;

        public BootstrapState(
            IGameStateMachine gameStateMachine,
            SceneLoader sceneLoader,
            AllServices services,
            UIHolder uiEntities,
            ICoroutineRunner coroutineRunner,
            ITicker ticker)
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _services = services;
            _UIEntities = uiEntities;
            _coroutineRunner = coroutineRunner;
            _ticker = ticker;

            RegisterServices(services);
        }

        public void Enter()
        {
            _sceneLoader.Load(InitialSceneName, OnLoaded);
        }

        public void Exit()
        {
            
        }

        private void OnLoaded()
        {
            _gameStateMachine.Enter<LoadProgressState>();
        }

        private void RegisterServices(AllServices services)
        {
            var staticData = RegisterStaticDataService(services);
            var assets = services.RegisterService<IAssets>(
                new AssetProvider());
            var inputService = services.RegisterService<IInputService>(
                InputService());
            var colorPicker = services.RegisterService<IColorPickerService>(
                new ColorPicker(staticData.ColorData));
            var persistentProgress = services.RegisterService<IPersistentProgressService>(
                new PersistentProgressService());
            var saveLoad = services.RegisterService <ISaveLoadService>(
                new SaveLoadService(persistentProgress));

            var blockPool = services.RegisterService<IBlockPool>(
                new BlockPool(assets, staticData.BlockData));
            var blockMovement = services.RegisterService<IBlockMovement>(
                new BlockMovement(staticData.BlockData));
            var UIFactory = services.RegisterService<IUIFactory>(
                new UIFactory(_UIEntities, assets, saveLoad, staticData.UIData));

            var blockFactory = RegisterFactory(blockPool, assets, colorPicker, blockMovement, staticData, inputService);

            var cameraMovement = services.RegisterService<ICameraMovement>(
                new CameraMovement(blockFactory, staticData.CameraData));
        }

        private IBlockFactory RegisterFactory(
            IBlockPool blockPool, 
            IAssets assets, 
            IColorPickerService colorPicker, 
            IBlockMovement blockMovement,
            IStaticDataService staticData,
            IInputService inputService)
        {
            IBlockFactory blockFactory = _services.RegisterService<IBlockFactory>(
                new BlockFactory(assets ,blockPool, colorPicker, 
                    blockMovement, staticData.BlockData));
         
            BlockParticle blockParticle = new BlockParticle(colorPicker, assets, staticData.ColorData, staticData.BlockData);
            blockParticle.Initialize();
            
            BlockStack _blockStack = new BlockStack();
            _blockStack.Construct(
                _ticker, blockPool, _gameStateMachine, inputService, blockFactory, 
                _UIEntities, blockMovement, blockParticle, staticData.BlockData);

            return blockFactory;
        }

        private IStaticDataService RegisterStaticDataService(AllServices services)
        {
            var staticDataService = new StaticDataService();
            staticDataService.Initialize();
            
            return services.RegisterService<IStaticDataService>(staticDataService);
        }
        
        private IInputService InputService() =>
            Application.isEditor
                ? new StandaloneInput()
                : new MobileInput();
    }
}