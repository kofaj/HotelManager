using HotelManager.Shared.Domain;

namespace HotelManager.Shared.Repositories;

internal class InMemoryBookingsRepository : IInMemoryRepository<Booking>
{
    private readonly List<Booking> _bookings = new();

    public void AddRange(IReadOnlyCollection<Booking> bookings)
    {
        _bookings.AddRange(bookings);
    }

    public Booking? GetById(string id)
    {
        throw new NotImplementedException();
    }
}
