using System;
using System.Collections;
using Infrastructure.Services;
using Infrastructure.States;
using UI.Factory;
using UnityEngine;

namespace Infrastructure
{
    public class Bootstrapper : Ticker, ICoroutineRunner
    {
        private void Awake()
        {
            SceneLoader sceneLoader = new SceneLoader(this);
            GameStateMachine gameStateMachine = new GameStateMachine(
                sceneLoader, AllServices.Container, UIHolder.Container, this, this);
            
            gameStateMachine.Enter<BootstrapState>();
            
            DontDestroyOnLoad(this);
        }

        public void DoAfter(Func<bool> condition, Action action) => 
            StartCoroutine(DoAfterCoroutine(condition, action));

        private IEnumerator DoAfterCoroutine(Func<bool> condition, Action action)
        {
            yield return new WaitUntil(condition);

            Debug.Log("Action");
            action.Invoke();
        }
    }
}
