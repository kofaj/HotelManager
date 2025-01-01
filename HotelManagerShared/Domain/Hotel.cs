namespace HotelManager.Shared.Domain;

public class Hotel
{
    /// <summary>
    /// Unique identifier for the hotel.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Name of the hotel.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// List of room types available in the hotel.
    /// </summary>
    public IReadOnlyCollection<RoomType> RoomTypes { get; }

    /// <summary>
    /// List of individual rooms in the hotel.
    /// </summary>
    public IReadOnlyCollection<Room> Rooms { get; }

    /// <summary>
    /// Constructor to initialize all properties.
    /// </summary>
    public Hotel(string id, string name, IReadOnlyCollection<RoomType> roomTypes, IReadOnlyCollection<Room> rooms)
    {
        Id = id;
        Name = name;
        RoomTypes = roomTypes;
        Rooms = rooms;
    }
}
