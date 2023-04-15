using Infrastructure.Factories;
using Infrastructure.Services.Input;
using UI.Entities;
using UI.Factory;
using UnityEngine;

namespace Infrastructure.States
{
    public class GameLoopState : IState
    {
        private readonly IBlockFactory _blockFactory;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IInputService _inputService;
        private readonly IUIEntitiesGetter _UIEntities;

        public bool IsPlaying { get; private set; }

        public GameLoopState(
            IBlockFactory blockFactory, 
            ICoroutineRunner coroutineRunner,
            IInputService inputService,
            IUIEntitiesGetter uiEntities)
        {
            _blockFactory = blockFactory;
            _coroutineRunner = coroutineRunner;
            _inputService = inputService;
            _UIEntities = uiEntities;
        }

        public void Enter()
        {
            _coroutineRunner.DoAfter(() => _inputService.Tap(), StartPlaying);
        }

        private void StartPlaying()
        {
            _blockFactory.CreateBlock();
            _UIEntities.Single<MainMenu>().Hide();
            _UIEntities.Single<ScoreContainer>().Show();

            IsPlaying = true;
        }

        public void Exit()
        {
            IsPlaying = false;
        }
    }
}