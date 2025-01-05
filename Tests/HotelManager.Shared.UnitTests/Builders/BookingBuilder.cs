using HotelManager.Shared.Domain;

namespace HotelManager.Shared.UnitTests.Builders;

public class BookingBuilder
{
    private string _hotelId;
    private DateOnly _arrival;
    private DateOnly _departure;
    private RoomType _roomType;
    private RoomRate _roomRate;

    /// <summary>
    /// Sets the hotel identifier.
    /// </summary>
    /// <param name="hotelId">The hotel ID.</param>
    /// <returns>The current instance of the builder.</returns>
    public BookingBuilder WithHotelId(string hotelId)
    {
        _hotelId = hotelId;
        return this;
    }

    /// <summary>
    /// Sets the arrival date.
    /// </summary>
    /// <param name="arrival">The arrival date.</param>
    /// <returns>The current instance of the builder.</returns>
    public BookingBuilder WithArrival(DateOnly arrival)
    {
        _arrival = arrival;
        return this;
    }

    /// <summary>
    /// Sets the departure date.
    /// </summary>
    /// <param name="departure">The departure date.</param>
    /// <returns>The current instance of the builder.</returns>
    public BookingBuilder WithDeparture(DateOnly departure)
    {
        _departure = departure;
        return this;
    }

    /// <summary>
    /// Sets the room type.
    /// </summary>
    /// <param name="roomType">The room type.</param>
    /// <returns>The current instance of the builder.</returns>
    public BookingBuilder WithRoomType(RoomType roomType)
    {
        _roomType = roomType;
        return this;
    }

    /// <summary>
    /// Sets the room rate.
    /// </summary>
    /// <param name="roomRate">The room rate.</param>
    /// <returns>The current instance of the builder.</returns>
    public BookingBuilder WithRoomRate(RoomRate roomRate)
    {
        _roomRate = roomRate;
        return this;
    }

    /// <summary>
    /// Builds the Booking instance.
    /// </summary>
    /// <returns>A new instance of the Booking class.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if any required property is not set.
    /// </exception>
    public Booking Build()
    {
        return new Booking(_hotelId, _arrival, _departure, _roomType, _roomRate);
    }

    public static BookingBuilder Init()
        => new();

    public static IReadOnlyCollection<Booking> BuildAsList(params Booking[] bookings)
        => [.. bookings];
}

