using HotelManager.Shared.Domain;

namespace HotelManager.Shared.Repositories;

internal class InMemoryBookingsRepository
{
    private readonly List<Booking> _bookings = new();

    public void Add(Booking booking)
    {
        // for the simplicity I'll keep the same model for Domain and DB object
        _bookings.Add(booking);
    }
}
