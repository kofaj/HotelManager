using HotelManager.Shared.Domain;
using HotelManager.Shared.Repositories;
using HotelManager.Shared.Services;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace HotelManager.Shared.Query;

public record AvailabilityQuery(string HotelId, DateOnly[] DateRange, RoomType RoomType) : IRequest<AvailabilityResult>;
public record AvailabilityResult(int RoomCount);


internal class AvailabilityQueryHandler : IRequestHandler<AvailabilityQuery, AvailabilityResult>
{
    private readonly IInMemoryRepository<Hotel> _hotelRepository;
    private readonly IInMemoryRepository<Booking> _bookingRepository;
    private readonly IDateProvider _dateTimeProvider;

    public AvailabilityQueryHandler(IInMemoryRepository<Hotel> hotelRepository, IInMemoryRepository<Booking> bookingRepository,
        IDateProvider dateTimeProvider)
    {
        _hotelRepository = hotelRepository;
        _bookingRepository = bookingRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    private void Validate(AvailabilityQuery command)
    {
        // Throwing exc is the easiest way to signalize that sth is wrong. It's ok for POC, however, later it can be replaced by the Result Type pattern

        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        var hotel = _hotelRepository.GetById(command.HotelId);
        if (hotel is null)
        {
            throw new ValidationException($"Hotel with id {command.HotelId} not found");
        }

        if (command.DateRange.Length == 1 && command.DateRange[0] < _dateTimeProvider.Today)
        {
            throw new ValidationException("Date cannot be in the past");
        }

        if (command.DateRange.Length == 2 && command.DateRange[0] > command.DateRange[1])
        {
            throw new ValidationException("Date range is invalid");
        }

        if (command.DateRange.Length > 1 && command.DateRange[0] == command.DateRange[1])
        {
            throw new ValidationException("Dates cannot be the same");
        }
    }

    public async Task<AvailabilityResult> Handle(AvailabilityQuery request, CancellationToken cancellationToken)
    {
        Validate(request);

        var hotel = _hotelRepository.GetById(request.HotelId) ?? throw new ArgumentException($"Hotel with ID {request.HotelId} not found.");
        var bookings = _bookingRepository.GetAll(x => x.HotelId == request.HotelId);

        int resultRoomCount;

        if (request.DateRange.Length == 1)
        {
            // Single date: calculate availability for one day
            var date = request.DateRange[0];
            var roomCount = hotel.GetAvailableRoomsCount(request.RoomType);
            var bookedCount = bookings.Count(w => w.IsAvailable(date));
            resultRoomCount = roomCount - bookedCount;
        }
        else
        {
            // Date range: sum available rooms across the range
            var startDate = request.DateRange[0];
            var endDate = request.DateRange[1];

            resultRoomCount = 0;

            for (var date = startDate; date < endDate; date = date.AddDays(1))
            {
                var roomCount = hotel.GetAvailableRoomsCount(request.RoomType);
                var bookedCount = bookings.Count(w => w.IsAvailable(date));
                resultRoomCount += roomCount - bookedCount;
            }
        }

        return new AvailabilityResult(resultRoomCount);
    }

}
