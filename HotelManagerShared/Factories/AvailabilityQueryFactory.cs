using HotelManager.Shared.Extensions;
using HotelManager.Shared.Queries;
using HotelManager.Shared.Services.JsonConverters;

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
            throw new ArgumentException("Hotel id is empty");
        }

        var dateRange = GetDates(parts[1]);
        var roomType = parts[2].GetRoomType();

        return new AvailabilityQuery(hotelId, dateRange, roomType);
    }

    private static DateOnly[] GetDates(string dateRange)
    {
        switch (dateRange.Length)
        {
            case 8:
            {
                var singleResult =
                    DateOnly.TryParseExact(dateRange, DateOnlyConverter.SerializationFormat, out var result);
                if (!singleResult)
                {
                    throw new ArgumentException($"Date is not recognized: {dateRange}");
                }

                return [result];
            }
            case 17:
            {
                var isRightSeparator = dateRange[8] == '-';
                if (!isRightSeparator)
                {
                    throw new ArgumentException($"Date Range separator is not recognized: {dateRange[8]}");
                }

                var firstDateResult = DateOnly.TryParseExact(dateRange[..8], DateOnlyConverter.SerializationFormat,
                    out var firstDate);
                if (!firstDateResult)
                {
                    throw new ArgumentException($"Date is not recognized: {dateRange[..8]}");
                }

                var lastDateResult = DateOnly.TryParseExact(dateRange[9..], DateOnlyConverter.SerializationFormat,
                    out var lastDate);
                if (!lastDateResult)
                {
                    throw new ArgumentException($"Date is not recognized: {dateRange[9..]}");
                }

                return [firstDate, lastDate];
            }
            default:
                throw new ArgumentException($"Date range is not recognized: {dateRange}");
        }
    }
}