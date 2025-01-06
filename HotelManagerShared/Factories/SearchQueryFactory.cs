using HotelManager.Shared.Query;
using HotelManager.Shared.Extensions;

namespace HotelManager.Shared.Factories;

public static class SearchQueryFactory
{
    public static SearchQuery Create(string command)
    {
        if(string.IsNullOrEmpty(command))
        {
            throw new ArgumentException("Command is empty");
        }
        
        var parts = command.Replace(" ", "").Split(',');
        if (parts.Length != 3)
        {
            throw new ArgumentException($"Command is not recognized: {command}");
        }

        var hotelId = parts[0];
        if (string.IsNullOrEmpty(hotelId))
        {
            throw new ArgumentException("Hotel id is empty");
        }

        var areDaysCountCorrect = int.TryParse(parts[1], out var daysCount);
        if (!areDaysCountCorrect)
        {
            throw new ArgumentException($"Days count is not recognized: {parts[1]}");
        }

        var roomType = parts[2].GetRoomType();

        return new SearchQuery(hotelId, daysCount, roomType);
    }
}