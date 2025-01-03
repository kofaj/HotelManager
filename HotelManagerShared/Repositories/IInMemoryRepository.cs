namespace HotelManager.Shared.Repositories;

// This interface was added only keep all Repositories with internal access and use them in the Services layer.
public interface IInMemoryRepository<TModel> where TModel : class
{
    public void AddRange(IReadOnlyCollection<TModel> models);
    public TModel? GetById(string id);
}
