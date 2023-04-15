using System.Collections;
using Gameplay.BlockFolder.Pool;
using Gameplay.СameraBehaviour;
using Infrastructure.Data.CameraData;
using Infrastructure.Services.Input;
using Infrastructure.Services.SaveLoad;
using UI.Entities;
using UI.Factory;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure.States
{
    public class GameLostState : IState
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IInputService _inputService;
        private readonly IUIFactory _UIFactory;
        private readonly IUIEntitiesGetter _UIEntities;
        private readonly ICameraMovement _cameraMovement;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IBlockPool _blockPool;

        private LoseWindow _loseWindow;
        private ScoreContainer _scoreContainer;

        private readonly float _timeBeforeLose;
        private readonly float _cameraRotateDuration;
        
        public GameLostState(
            IGameStateMachine gameStateMachine,
            IUIFactory UIFactory,
            IBlockPool blockPool,
            IInputService inputService,
            IUIEntitiesGetter uiEntities, 
            ICameraMovement cameraMovement,
            ICoroutineRunner coroutineRunner,
            CameraStaticData cameraData)
        {
            _gameStateMachine = gameStateMachine;
            _UIFactory = UIFactory;
            _blockPool = blockPool;
            _inputService = inputService;
            _UIEntities = uiEntities;
            _cameraMovement = cameraMovement;
            _coroutineRunner = coroutineRunner;

            _timeBeforeLose = cameraData.TimeBeforeLose;
            _cameraRotateDuration = cameraData.LoseRotationDuration;
        }

        public void Enter()
        {
            _scoreContainer = GetScoreContainer();
            _coroutineRunner.StartCoroutine(LoseAfter(_timeBeforeLose));
        }

        public void Exit()
        {
            _loseWindow.Hide();
            _scoreContainer.ResetScore();
        }

        private void ShowLoseWindow()
        {
            _loseWindow ??= _UIFactory.CreateLoseWindow();

            _loseWindow.Show();
            _loseWindow.SetScore(_scoreContainer.Score, _scoreContainer.HighestScore);
        }

        private ScoreContainer GetScoreContainer() => 
            _UIEntities.Single<ScoreContainer>();

        private void RestartGame()
        {
            _gameStateMachine.Enter<LoadLevelState, string>(SceneManager.GetActiveScene().name);
        }

        private void ReleaseAllBlocks()
        {
            foreach (var block in _blockPool.ActiveBlocks)
                block.Release();
        }

        private IEnumerator LoseAfter(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            
            _scoreContainer.Hide();
            
            _cameraMovement.LoseRotate(ShowLoseWindow);

            yield return new WaitForSecondsRealtime(_cameraRotateDuration);

            ReleaseAllBlocks();
            _coroutineRunner.DoAfter(() => _inputService.Tap(), RestartGame);
        }
    }
}