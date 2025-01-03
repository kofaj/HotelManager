using System.Text.Json.Serialization;

namespace HotelManager.Shared.Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RoomType
{
    SGL,
    DBL
}
