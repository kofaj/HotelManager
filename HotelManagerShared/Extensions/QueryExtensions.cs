using HotelManager.Shared.Domain;

namespace HotelManager.Shared.Extensions;

internal static class QueryExtensions
{
    public static RoomType GetRoomType(this string roomType)
    {
        var parseResult = Enum.TryParse<RoomType>(roomType, out var result);
        if (!parseResult)
        {
            throw new ArgumentException($"Room Type is not recognized: {roomType}");
        }

        return result;
    }
}