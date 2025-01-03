using HotelManager.Shared.Domain;

namespace HotelManager.Shared.Repositories;

internal class InMemoryHotelsRepository : IInMemoryRepository<Hotel>
{
    private readonly List<Hotel> _hotels = new();

    public void AddRange(IReadOnlyCollection<Hotel> hotels)
    {
        _hotels.AddRange(hotels);
    }

    public Hotel? GetById(string id)
        => _hotels.FirstOrDefault(x => x.Id == id);
}
