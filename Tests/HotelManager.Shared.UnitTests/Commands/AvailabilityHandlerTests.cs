using FluentAssertions;
using HotelManager.Shared.Commands;
using HotelManager.Shared.Domain;
using HotelManager.Shared.Repositories;
using HotelManager.Shared.UnitTests.Builders;
using Moq;

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

        var hotel = HotelBuilder.Init()
            .SetId(hotelId)
            .SetName("Hotel")
            .AddRoom(new Room(RoomType, "1"))
            .AddRoom(new Room(RoomType, "2"))
            .AddRoom(new Room(RoomType.SGL, "3"))
            .Build();

        _hotelRepositoryMock.Setup(x => x.GetById("1")).Returns(hotel);

        // Act
        var result = await _handler.Handle(new AvailabilityCommand(hotelId, "20240901-20240903", RoomType.ToString()), default);

        // Assert
        result.Should().NotBeNull();
        result.Hotel.Id.Should().Be(hotel.Id);
        result.RoomCount.Should().Be(2);
    }
}
