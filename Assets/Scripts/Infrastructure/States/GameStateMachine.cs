using System;
using System.Collections.Generic;
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

namespace Infrastructure.States
{
    public class GameStateMachine : IGameStateMachine
    {
        private readonly Dictionary<Type, IExitState> _states;
        private IExitState _currentState;
        
        public bool IsInGameLoop
        {
            get
            {
                GameLoopState gameLoopState = (GameLoopState)_states[typeof(GameLoopState)];
                return gameLoopState.IsPlaying;
            }
        }

        public GameStateMachine(
            SceneLoader sceneLoader, 
            AllServices services,
            UIHolder UIHolder,
            ITicker ticker,
            ICoroutineRunner coroutineRunner)
        {
            _states = new Dictionary<Type, IExitState>
            {
                [typeof(BootstrapState)] = new BootstrapState(
                    this, sceneLoader, services, UIHolder, coroutineRunner, ticker),
                [typeof(LoadLevelState)] = new LoadLevelState(
                    this , services.Single<ISaveLoadService>(), sceneLoader, services.Single<ICameraMovement>(),
                    services.Single<IBlockFactory>(), services.Single<IUIFactory>(), UIHolder, 
                    coroutineRunner, services.Single<IAssets>(), services.Single<IColorPickerService>(),
                    services.Single<IStaticDataService>()),
                [typeof(LoadProgressState)] = new LoadProgressState(
                    this, services.Single<IPersistentProgressService>(), 
                    services.Single<ISaveLoadService>()),
                [typeof(GameLoopState)] = new GameLoopState(
                    services.Single<IBlockFactory>(), coroutineRunner, 
                    services.Single<IInputService>(), UIHolder),
                [typeof(GameLostState)] = new GameLostState(
                    this ,services.Single<IUIFactory>(), services.Single<IBlockPool>(),
                    services.Single<IInputService>(), UIHolder, services.Single<ICameraMovement>(), 
                    coroutineRunner, services.Single<IStaticDataService>().CameraData)
            };
        }

        public void Enter<TState>() where TState : class, IState
        {
            IState state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
        {
            TState state = ChangeState<TState>();
            state.Enter(payload);
        }

        private TState ChangeState<TState>() where TState : class, IExitState
        {
            _currentState?.Exit();

            TState state = GetState<TState>();
            _currentState = state;

            return state;
        }

        private TState GetState<TState>() where TState : class, IExitState => 
            _states[typeof(TState)] as TState;
    }
}