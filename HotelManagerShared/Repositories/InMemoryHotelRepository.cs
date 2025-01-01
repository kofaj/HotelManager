using HotelManager.Shared.Domain;

namespace HotelManager.Shared.Repositories;

internal class InMemoryHotelRepository
{
    private readonly List<Hotel> _hotels = new();

    public void Add(Hotel hotel)
    {
        _hotels.Add(hotel);
    }

    public Hotel? GetById(string id)
        => _hotels.FirstOrDefault(x => x.Id == id);
}
