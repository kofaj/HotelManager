using HotelManager.Shared.Domain;
using HotelManager.Shared.Repositories;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace HotelManager.Shared.Commands;

public record AvailabilityCommand(string HotelId, DateOnly[] DateRange, RoomType RoomType) : IRequest<AvailabilityResult>;

public record AvailabilityResult(Hotel Hotel, int RoomCount);

internal class AvailabilityHandler : IRequestHandler<AvailabilityCommand, AvailabilityResult>
{
    private readonly IInMemoryRepository<Hotel> _hotelRepository;
    private readonly IInMemoryRepository<Booking> _bookingRepository;

    public AvailabilityHandler(IInMemoryRepository<Hotel> hotelRepository, IInMemoryRepository<Booking> bookingRepository)
    {
        _hotelRepository = hotelRepository;
        _bookingRepository = bookingRepository;
    }

    private void Validate(AvailabilityCommand command)
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

        if (command.DateRange.Length == 1 && command.DateRange[0] < DateOnly.FromDateTime(DateTime.Now))
        {
            throw new ValidationException("Date cannot be in the past");
        }

        if (command.DateRange.Length == 2 && command.DateRange[0] > command.DateRange[1])
        {
            throw new ValidationException("Date range is invalid");
        }

        if (command.DateRange[0] == command.DateRange[1])
        {
            throw new ValidationException("Dates cannot be the same");
        }
    }

    public async Task<AvailabilityResult> Handle(AvailabilityCommand request, CancellationToken cancellationToken)
    {
        Validate(request);

        var hotel = _hotelRepository.GetById(request.HotelId);
        var roomCount = hotel!.GetAvailableRoomsCount(request.RoomType);
        var bookings = _bookingRepository.GetAll(x => x.HotelId == request.HotelId);

        var resultRoomCount = roomCount - (request.DateRange.Length == 1
            ? bookings.Count(w => w.IsAvailable(request.DateRange[0]))
            : bookings.Count(w => w.IsAvailable(request.DateRange[0], request.DateRange[1])));

        return new AvailabilityResult(hotel, resultRoomCount);
    }
}
