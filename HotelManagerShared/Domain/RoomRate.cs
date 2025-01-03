using System.Text.Json.Serialization;

namespace HotelManager.Shared.Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RoomRate
{
    Prepaid,
    Standard
}
