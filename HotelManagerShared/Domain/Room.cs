using System.Text.Json.Serialization;

namespace HotelManager.Shared.Domain;

public class Room
{
    /// <summary>
    /// The type of the room (e.g., SGL, DBL).
    /// </summary>
    public RoomType RoomType { get; }

    /// <summary>
    /// Unique identifier for the room.
    /// </summary>
    public string RoomId { get; }

    /// <summary>
    /// Constructor to initialize all properties.
    /// </summary>
    public Room(RoomType roomType, string roomId)
    {
        RoomType = roomType;
        RoomId = roomId;
    }
}
