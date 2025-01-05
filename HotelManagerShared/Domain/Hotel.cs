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
    private IReadOnlyCollection<RoomDetails> RoomTypes { get; }

    /// <summary>
    /// List of individual rooms in the hotel.
    /// </summary>
    private IReadOnlyCollection<Room> Rooms { get; }

    /// <summary>
    /// Constructor to initialize all properties.
    /// </summary>
    public Hotel(string id, string name, IReadOnlyCollection<RoomDetails> roomTypes, IReadOnlyCollection<Room> rooms)
    {
        Id = id;
        Name = name;
        RoomTypes = roomTypes;
        Rooms = rooms;
    }

    /// <summary>
    /// Constructor for System.Text.Json.
    /// </summary>
    public Hotel()
    {

    }

    public int GetAvailableRoomsCount(RoomType roomType)
        => Rooms.Count(r => r.RoomType == roomType);
}
