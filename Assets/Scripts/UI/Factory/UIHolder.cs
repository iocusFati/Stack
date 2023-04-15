using Infrastructure.Services;

namespace UI.Factory
{
    public class UIHolder : IUIEntitiesGetter
    {
        private static UIHolder _instance;
        public static UIHolder Container => _instance ??= new UIHolder();

        public TUIEntity RegisterUIEntity<TUIEntity>(TUIEntity implementation) where TUIEntity : IUIEntity =>
            Implementation<TUIEntity>.UIEntityInstance = implementation;
        
        public TUIEntity Single<TUIEntity>() where TUIEntity : IUIEntity =>
            Implementation<TUIEntity>.UIEntityInstance;

        private class Implementation<TUIEntity> where TUIEntity : IUIEntity
        {
            public static TUIEntity UIEntityInstance;
        }
    }
}