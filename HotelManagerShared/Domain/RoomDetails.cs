namespace HotelManager.Shared.Domain;

public class RoomDetails
{
    /// <summary>
    /// Code representing the room type (e.g., SGL, DBL).
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Description of the room type.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// List of amenities provided with this room type.
    /// </summary>
    public List<string> Amenities { get; set; }

    /// <summary>
    /// List of special features for this room type.
    /// </summary>
    public List<string> Features { get; set; }

    /// <summary>
    /// Constructor to initialize all properties.
    /// </summary>
    public RoomDetails(string code, string description, List<string> amenities, List<string> features)
    {
        Code = code;
        Description = description;
        Amenities = amenities;
        Features = features;
    }
}
