namespace DavidStudio.Core.DataIO.Entities;

public interface ISelfManageable<TEntity>
{
    public static abstract TEntity Create(TEntity model);
    public void Update(TEntity model);
}

public interface ISelfManageable<out TEntity, in TCreateModel, in TUpdateModel>
{
    public static abstract TEntity Create(TCreateModel model);
    public void Update(TUpdateModel model);
}