using HotelManager.Shared.Query;
using HotelManager.Shared.Extensions;
using System.ComponentModel.DataAnnotations;

namespace HotelManager.Shared.Factories;

public static class AvailabilityQueryFactory
{
    public static AvailabilityQuery Create(string command)
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

        var dateRange = GetDates(parts[1]);
        var roomType = parts[2].GetRoomType();

        return new AvailabilityQuery(hotelId, dateRange, roomType);
    }

    private static DateOnly[] GetDates(string dateRange)
    {
        if (dateRange.Length == 8)
        {
            var singleResult = DateOnly.TryParse(dateRange, out var result);
            if (!singleResult)
            {
                throw new ArgumentException($"Date is not recognized: {dateRange}");
            }

            return [result];
        }

        if (dateRange.Length == 17)
        {
            var isRightSeparator = dateRange[8] == '-';
            if (!isRightSeparator)
            {
                throw new ArgumentException($"Date Range separator is not recognized: {dateRange[8]}");
            }

            var firstDateResult = DateOnly.TryParse(dateRange[..7], out var firstDate);
            if (!firstDateResult)
            {
                throw new ArgumentException($"Date is not recognized: {dateRange[..7]}");
            }

            var lastDateResult = DateOnly.TryParse(dateRange.Substring(8, 7), out var lastDate);
            if (!lastDateResult)
            {
                throw new ArgumentException($"Date is not recognized: {dateRange.Substring(8, 7)}");
            }

            return [firstDate, lastDate];
        }


        throw new ArgumentException($"Date range is not recognized: {dateRange}");
    }

}
