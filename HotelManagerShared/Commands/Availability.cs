using HotelManager.Shared.Domain;
using HotelManager.Shared.Repositories;
using MediatR;

namespace HotelManager.Shared.Commands;

public record AvailabilityCommand(string HotelId, string DateRange, string RoomType) : IRequest<Hotel>
{
    public static AvailabilityCommand Create(string command)
    {
        var parts = command.Replace(" ", "").Split(',');
        return new AvailabilityCommand(parts[1], parts[2], parts[3]);
    }
};

internal class AvailabilityHandler : IRequestHandler<AvailabilityCommand, Hotel>
{
    private readonly InMemoryHotelsRepository _repository;

    public AvailabilityHandler(InMemoryHotelsRepository repository)
    {
        _repository = repository;
    }

    public void Handle(AvailabilityCommand command)
    {
        Validate(command);

        // we can add hotelId validation later
        var hotelId = command.HotelId;
        var dateRange = GetDates(command.DateRange);
        var roomType = GetRoomType(command.RoomType);

        var hotel = _repository.GetById(hotelId);


        // return hotel.
    }

    private void Validate(AvailabilityCommand command)
    {
        // Throwing exc is the easiest way to signalize that sth is wrong. It's ok for POC, however, later it can be replaced by the Result Type pattern

        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        if (string.IsNullOrEmpty(command.DateRange))
        {
            throw new ArgumentNullException(nameof(command.DateRange));
        }

        if (string.IsNullOrEmpty(command.HotelId))
        {
            throw new ArgumentNullException(nameof(command.HotelId));
        }

        if (string.IsNullOrEmpty(command.RoomType))
        {
            throw new ArgumentNullException(nameof(command.RoomType));
        }
    }

    private static RoomType GetRoomType(string roomType)
    {
        var parseResult = Enum.TryParse<RoomType>(roomType, out var result);
        if (!parseResult)
        {
            throw new ArgumentException($"Room Type is not recognized: {roomType}");
        }

        return result;
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

    public Task<Hotel> Handle(AvailabilityCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
