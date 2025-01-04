using HotelManager.Shared.Domain;

namespace HotelManager.Shared.Repositories;

internal class InMemoryBookingsRepository : IInMemoryRepository<Booking>
{
    private readonly List<Booking> _bookings = new();

    public void AddRange(IReadOnlyCollection<Booking> bookings)
    {
        _bookings.AddRange(bookings);
    }

    public IReadOnlyCollection<Booking> GetAll(Func<Booking, bool> predicate)
        => _bookings?.Where(predicate)?.ToList() ?? new List<Booking>();

    public Booking? GetById(string id)
    {
        throw new NotImplementedException();
    }
}
