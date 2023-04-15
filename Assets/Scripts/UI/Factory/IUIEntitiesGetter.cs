namespace UI.Factory
{
    public interface IUIEntitiesGetter
    {
        TUIEntity Single<TUIEntity>() where TUIEntity : IUIEntity;
    }
}