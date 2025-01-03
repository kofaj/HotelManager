namespace HotelManager.Shared.Domain;

public class Booking
{
    /// <summary>
    /// Unique identifier for the hotel.
    /// </summary>
    public string HotelId { get; }

    /// <summary>
    /// The arrival date in the format YYYYMMDD.
    /// </summary>
    public DateOnly Arrival { get; }

    /// <summary>
    /// The departure date in the format YYYYMMDD.
    /// </summary>
    public DateOnly Departure { get; }

    /// <summary>
    /// Type of the room (e.g., DBL, SGL).
    /// </summary>
    public RoomType RoomType { get; }

    /// <summary>
    /// The rate type for the room (e.g., Prepaid, Standard).
    /// </summary>
    public RoomRate RoomRate { get; }

    public Booking(string hotelId, DateOnly arrival, DateOnly departure, RoomType roomType, RoomRate roomRate)
    {
        HotelId = hotelId;
        Arrival = arrival;
        Departure = departure;
        RoomType = roomType;
        RoomRate = roomRate;
    }
}
