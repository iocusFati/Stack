using Infrastructure.AssetProviderService;
using Infrastructure.Data.UIData;
using Infrastructure.Services.SaveLoad;
using Infrastructure.States;
using UI.Entities;
using UnityEngine;

namespace UI.Factory
{
    public class UIFactory : IUIFactory
    {
        private readonly IAssets _assetProvider;
        private readonly UIStaticData _UIStaticData;
        private readonly UIHolder _UIContainer;
        private readonly ISaveLoadService _saveLoad;

        private Canvas _baseRoot;

        public UIFactory(
            UIHolder container,
            IAssets assetProvider,
            ISaveLoadService saveLoad,
            UIStaticData UIStaticData)
        {
            _UIContainer = container;
            _assetProvider = assetProvider;
            _saveLoad = saveLoad;
            _UIStaticData = UIStaticData;
        }

        public MainMenu CreateMainMenu()
        {
            MainMenu menu = _assetProvider.Instantiate<MainMenu>(AssetPaths.MainMenu, _baseRoot.transform);
            
            _UIContainer.RegisterUIEntity(menu);

            return menu;
        }

        public LoseWindow CreateLoseWindow()
        {
            LoseWindow window = _assetProvider.Instantiate<LoseWindow>(AssetPaths.LoseWindow, _baseRoot.transform);
            window.Construct(_UIStaticData);
            _UIContainer.RegisterUIEntity(window);
            // window.transform.SetParent(_baseRoot.transform);
            
            return window;
        }

        public ScoreContainer CreateScoreContainer()
        {
            Transform root = CreateUIRoot().transform;
            ScoreContainer container = InstantiateRegistered<ScoreContainer>(AssetPaths.ScoreContainer, root);
            
            container.transform.SetParent(root, false);
            
            _UIContainer.RegisterUIEntity(container);
            return container;
        }

        public void CreateBaseUIRoot() => 
            _baseRoot = CreateUIRoot();

        private Canvas CreateUIRoot(int order = 0)
        {
            Canvas canvas = _assetProvider.Instantiate<Canvas>(AssetPaths.UIRoot);
            canvas.sortingOrder = order;

            return canvas;
        }

        private TSavedProgress InstantiateRegistered<TSavedProgress>(string path, Transform parent) 
            where TSavedProgress : Object, ISavedProgressReader
        {
            TSavedProgress instantiated = _assetProvider.Instantiate<TSavedProgress>(path, parent);
            _saveLoad.Register(instantiated);

            return instantiated;
        }
    }
}