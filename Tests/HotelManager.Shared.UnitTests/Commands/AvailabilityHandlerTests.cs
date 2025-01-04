﻿using FluentAssertions;
using HotelManager.Shared.Commands;
using HotelManager.Shared.Domain;
using HotelManager.Shared.Repositories;
using HotelManager.Shared.Services;
using HotelManager.Shared.UnitTests.Builders;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace HotelManager.Shared.UnitTests.Commands;

public class AvailabilityHandlerTests
{
    private readonly AvailabilityHandler _handler;
    private readonly Mock<IInMemoryRepository<Booking>> _bookingRepositoryMock;
    private readonly Mock<IInMemoryRepository<Hotel>> _hotelRepositoryMock;
    private readonly Mock<IDateProvider> _dateProvider;
    private readonly RoomType RoomType = RoomType.DBL;

    public AvailabilityHandlerTests()
    {
        _bookingRepositoryMock = new Mock<IInMemoryRepository<Booking>>();
        _hotelRepositoryMock = new Mock<IInMemoryRepository<Hotel>>();
        _dateProvider = new Mock<IDateProvider>();
        _dateProvider.Setup(x => x.Today).Returns(DateOnly.FromDateTime(DateTime.Now));

        _handler = new AvailabilityHandler(_hotelRepositoryMock.Object, _bookingRepositoryMock.Object, _dateProvider.Object);
    }

    [Fact]
    public async Task CheckAvailabilityWhenDateIsSingleAndThereIsNoBookingsShouldReturnAllRoomsCount()
    {
        // Arrange
        const string hotelId = "1";
        var dates = new DateOnly[] { (DateOnly.FromDateTime(DateTime.Now.AddDays(2))) };

        var hotel = AddDefaultHotelToRepository(hotelId);
        _bookingRepositoryMock.Setup(x => x.GetAll(It.IsAny<Func<Booking, bool>>())).Returns(new List<Booking>());

        // Act
        var result = await _handler.Handle(new AvailabilityCommand(hotelId, dates, RoomType), default);

        // Assert
        result.Should().NotBeNull();
        result.Hotel.Id.Should().Be(hotel.Id);
        result.RoomCount.Should().Be(2);
    }

    [Fact]
    public async Task CheckAvailabilityWhenMultiDateAndThereIsNoBookingsShouldReturnAllRoomsCount()
    {
        // Arrange
        const string hotelId = "1";
        var dates = new DateOnly[] { (DateOnly.FromDateTime(DateTime.Now.AddDays(2))), (DateOnly.FromDateTime(DateTime.Now.AddDays(8))) };
        var hotel = AddDefaultHotelToRepository(hotelId);

        _bookingRepositoryMock.Setup(x => x.GetAll(It.IsAny<Func<Booking, bool>>())).Returns(new List<Booking>());

        // Act
        var result = await _handler.Handle(new AvailabilityCommand(hotelId, dates, RoomType), default);

        // Assert
        result.Should().NotBeNull();
        result.Hotel.Id.Should().Be(hotel.Id);
        result.RoomCount.Should().Be(2);
    }

    [Fact]
    public async Task CheckAvailabilityWhenHotelIdIsIncorrectThenShouldThrowException()
    {
        // Arrange
        const string hotelId = "1";
        var dates = new DateOnly[] { (DateOnly.FromDateTime(DateTime.Now.AddDays(2))) };

        _hotelRepositoryMock.Setup(x => x.GetById("1")).Returns((Hotel)null!);

        // Act
        Func<Task> act = async () => await _handler.Handle(new AvailabilityCommand(hotelId, dates, RoomType), default);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Theory]
    [InlineData(5, 5)]
    [InlineData(5, null)]
    [InlineData(-5, null)]
    [InlineData(null, null)]
    public async Task CheckAvailabilityWhenIncorrectThenShouldThrowException(int? startDaysFromToday, int? endDaysFromToday)
    {
        // Arrange
        var startDateOnly = startDaysFromToday is not null ? DateOnly.FromDateTime(DateTime.Now.AddDays(startDaysFromToday.Value)) : DateOnly.MinValue;
        var endDateOnly = endDaysFromToday is not null ? DateOnly.FromDateTime(DateTime.Now.AddDays(endDaysFromToday!.Value)) : DateOnly.MinValue;
        var hotel = AddDefaultHotelToRepository("1");

        // Act
        Func<Task> act = async () => await _handler.Handle(new AvailabilityCommand(hotel.Id, [startDateOnly, endDateOnly], RoomType), default);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    private Hotel AddDefaultHotelToRepository(string hotelId)
    {
        var hotel = HotelBuilder.Init()
           .SetId(hotelId)
           .SetName("Hotel")
           .AddRoom(new Room(RoomType, "1"))
           .AddRoom(new Room(RoomType, "2"))
           .AddRoom(new Room(RoomType.SGL, "3"))
           .Build();

        _hotelRepositoryMock.Setup(x => x.GetById("1")).Returns(hotel);

        return hotel;
    }
}
