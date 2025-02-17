﻿using System.ComponentModel.DataAnnotations;
using HotelManager.Shared.Domain;
using HotelManager.Shared.Repositories;
using HotelManager.Shared.Services;
using MediatR;

namespace HotelManager.Shared.Queries;

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
        ArgumentNullException.ThrowIfNull(command);

        var hotel = _hotelRepository.GetById(command.HotelId);
        if (hotel is null)
        {
            throw new ValidationException($"Hotel with id {command.HotelId} not found");
        }

        switch (command.DateRange.Length)
        {
            case 1 when command.DateRange[0] < _dateTimeProvider.Today:
                throw new ValidationException("Date cannot be in the past");
            case 2 when command.DateRange[0] > command.DateRange[1]:
                throw new ValidationException("Date range is invalid");
            case > 1 when command.DateRange[0] == command.DateRange[1]:
                throw new ValidationException("Dates cannot be the same");
        }
    }

    public async Task<AvailabilityResult> Handle(AvailabilityQuery request, CancellationToken cancellationToken)
    {
        Validate(request);

        var hotel = _hotelRepository.GetById(request.HotelId) ?? throw new ArgumentException($"Hotel with ID {request.HotelId} not found.");
        var roomCount = hotel!.GetAvailableRoomsCount(request.RoomType);
        var bookings = _bookingRepository.GetAll(x => x.HotelId == request.HotelId);

        var resultRoomCount = roomCount - (request.DateRange.Length == 1
            ? bookings.Count(w => w.IsAvailable(request.DateRange[0]))
            : bookings.Count(w => w.IsAvailable(request.DateRange[0], request.DateRange[1])));

        return new AvailabilityResult(resultRoomCount);
    }
}
