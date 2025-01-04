using FluentAssertions;
using HotelManager.Shared.Commands;
using HotelManager.Shared.Domain;
using HotelManager.Shared.Repositories;
using HotelManager.Shared.UnitTests.Builders;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace HotelManager.Shared.UnitTests.Commands;

public class AvailabilityHandlerTests
{
    private readonly AvailabilityHandler _handler;
    private readonly Mock<IInMemoryRepository<Booking>> _bookingRepositoryMock;
    private readonly Mock<IInMemoryRepository<Hotel>> _hotelRepositoryMock;
    private readonly RoomType RoomType = RoomType.DBL;

    public AvailabilityHandlerTests()
    {
        _bookingRepositoryMock = new Mock<IInMemoryRepository<Booking>>();
        _hotelRepositoryMock = new Mock<IInMemoryRepository<Hotel>>();
        _handler = new AvailabilityHandler(_hotelRepositoryMock.Object, _bookingRepositoryMock.Object);
    }

    [Fact]
    public async Task CheckAvailabilityWhenDateIsSingleAndThereIsNoBookingsShouldReturnAllRoomsCount()
    {
        // Arrange
        const string hotelId = "1";
        var dates = new DateOnly[] { new(2024, 09, 01) };

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
        var dates = new DateOnly[] { new(2024, 09, 01), new(2024, 09, 03) };
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
        var dates = new DateOnly[] { new(2024, 09, 01) };

        _hotelRepositoryMock.Setup(x => x.GetById("1")).Returns((Hotel)null!);

        // Act
        Func<Task> act = async () => await _handler.Handle(new AvailabilityCommand(hotelId, dates, RoomType), default);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Theory]
    [InlineData("2024-09-03", "2024-09-03")]
    [InlineData("2023-09-03", null)]
    [InlineData(null, null)]
    public async Task CheckAvailabilityWhenIncorrectThenShouldThrowException(string startDate, string endDate)
    {
        // Arrange
        var startDateOnly = startDate is not null ? DateOnly.Parse(startDate) : DateOnly.MinValue;
        var endDateOnly = endDate is not null ? DateOnly.Parse(endDate) : DateOnly.MinValue;
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
