using HotelManager.Shared.Query;
using HotelManager.Shared.Extensions;
using System.ComponentModel.DataAnnotations;

namespace HotelManager.Shared.Factories;

public class SearchQueryFactory
{
    public static SearchQuery Create(string command)
    {
        var parts = command.Replace(" ", "").Split(',');
        if (parts.Length != 3)
        {
            throw new ArgumentException($"Command is not recognized: {command}");
        }

        var hotelId = parts[0];
        if (string.IsNullOrEmpty(hotelId))
        {
            throw new ValidationException("Hotel id is empty");
        }

        var areDaysCountCorrect = int.TryParse(parts[1], out var daysCount);
        if (!areDaysCountCorrect)
        {
            throw new ValidationException($"Days count is not recognized: {parts[1]}");
        }

        var roomType = parts[2].GetRoomType();

        return new SearchQuery(hotelId, daysCount, roomType);
    }
}
