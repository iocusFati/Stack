using Infrastructure.Services;
using Infrastructure.States;
using UI.Entities;
using UnityEngine;

namespace UI.Factory
{
    public interface IUIFactory : IService
    {
        void CreateBaseUIRoot();
        ScoreContainer CreateScoreContainer();
        LoseWindow CreateLoseWindow();
        MainMenu CreateMainMenu();
    }
}