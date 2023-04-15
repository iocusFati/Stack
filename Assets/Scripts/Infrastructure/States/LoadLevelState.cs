using Gameplay.ColorPicker;
using Gameplay.СameraBehaviour;
using Infrastructure.AssetProviderService;
using Infrastructure.Factories;
using Infrastructure.Services.SaveLoad;
using Infrastructure.Services.StaticDataService;
using UI.Entities;
using UI.Factory;
using UnityEngine;
using UnityEngine.SceneManagement;
using Skybox = DefaultNamespace.Environment.SkyboxFolder.Skybox;

namespace Infrastructure.States
{
    public class LoadLevelState : IPayloadedState<string>
    {
        private readonly ISaveLoadService _saveLoadService;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly IBlockFactory _blockFactory;
        private readonly ICameraMovement _cameraMovement;
        private readonly IUIFactory _UIFactory;
        private readonly IUIEntitiesGetter _UIEntitiesGetter;
        private readonly IStaticDataService _staticData;
        private readonly IAssets _assetProvider;
        private readonly IColorPickerService _colorPicker;
        private readonly ICoroutineRunner _coroutineRunner;

        private Vector3 _initialPoint;
        private ScoreContainer _scoreContainer;

        public LoadLevelState(
            IGameStateMachine gameStateMachine,
            ISaveLoadService saveLoadService,
            SceneLoader sceneLoader,
            ICameraMovement cameraMovement,
            IBlockFactory blockFactory,
            IUIFactory UIFactory,
            IUIEntitiesGetter UIEntitiesGetter,
            ICoroutineRunner coroutineRunner,
            IAssets assets, 
            IColorPickerService colorPicker,
            IStaticDataService staticDataService)
        {
            _gameStateMachine = gameStateMachine;
            _saveLoadService = saveLoadService;
            _sceneLoader = sceneLoader;
            _cameraMovement = cameraMovement;
            _blockFactory = blockFactory;
            _UIFactory = UIFactory;
            _UIEntitiesGetter = UIEntitiesGetter;
            _coroutineRunner = coroutineRunner;
            _assetProvider = assets;
            _colorPicker = colorPicker;
            _staticData = staticDataService;
        }
        public void Enter(string sceneName)
        {
            if (sceneName != SceneManager.GetActiveScene().name)
            {
                _sceneLoader.Load(sceneName, OnLoaded);
            }
            else
            {
                Reload();
            }
        }

        public void Exit()
        {
            
        }

        private void OnLoaded()
        {
            _cameraMovement.SetMainCamera();
            _blockFactory.CreateFirstBlock();
            
            _UIFactory.CreateBaseUIRoot();
            _UIFactory.CreateMainMenu();
            _scoreContainer = _UIFactory.CreateScoreContainer();
            _scoreContainer.Hide();

            Skybox skybox = _assetProvider.Instantiate<Skybox>(AssetPaths.Skybox);
            skybox.Construct(_colorPicker, _blockFactory, _coroutineRunner, _staticData.ColorData);
            _saveLoadService.InformReaders();
            _gameStateMachine.Enter<GameLoopState>();
        }

        private void Reload()
        {
            _cameraMovement.PosYToInitial();
            _UIEntitiesGetter.Single<MainMenu>().Show();
            _cameraMovement.LoseRotate(
                () => _blockFactory
                    .CreateFirstBlock(true, OnFirstBlockCreated), true);
        }

        private void OnFirstBlockCreated()
        {
            _gameStateMachine.Enter<GameLoopState>();
        }
    }
}