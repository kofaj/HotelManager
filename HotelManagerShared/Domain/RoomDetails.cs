namespace HotelManager.Shared.Domain;

public class RoomDetails
{
    /// <summary>
    /// Code representing the room type (e.g., SGL, DBL).
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Description of the room type.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// List of amenities provided with this room type.
    /// </summary>
    public IReadOnlyCollection<string> Amenities { get; }

    /// <summary>
    /// List of special features for this room type.
    /// </summary>
    public IReadOnlyCollection<string> Features { get; }

    /// <summary>
    /// Constructor to initialize all properties.
    /// </summary>
    public RoomDetails(string code, string description, IReadOnlyCollection<string> amenities, IReadOnlyCollection<string> features)
    {
        Code = code;
        Description = description;
        Amenities = amenities;
        Features = features;
    }
}
