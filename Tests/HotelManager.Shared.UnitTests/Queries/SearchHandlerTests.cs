using FluentAssertions;
using HotelManager.Shared.Commands;
using HotelManager.Shared.Domain;
using HotelManager.Shared.Repositories;
using HotelManager.Shared.Services;
using HotelManager.Shared.UnitTests.Builders;
using Moq;

namespace HotelManager.Shared.UnitTests.Queries;

public class SearchHandlerTests
{
    private readonly SearchQueryHandler _handler;
    private readonly Mock<IInMemoryRepository<Booking>> _bookingRepositoryMock;
    private readonly Mock<IInMemoryRepository<Hotel>> _hotelRepositoryMock;
    private readonly Mock<IDateProvider> _dateProvider;
    private readonly RoomType RoomType = RoomType.DBL;

    public SearchHandlerTests()
    {
        _bookingRepositoryMock = new Mock<IInMemoryRepository<Booking>>();
        _hotelRepositoryMock = new Mock<IInMemoryRepository<Hotel>>();
        _dateProvider = new Mock<IDateProvider>();
        _dateProvider.Setup(x => x.Today).Returns(DateOnly.FromDateTime(DateTime.Now));

        _handler = new SearchQueryHandler(_hotelRepositoryMock.Object, _bookingRepositoryMock.Object, _dateProvider.Object);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public async Task HandleQueryWhenRoomHasNoBookingsThenShouldReturnFullListOfAvailableBookins(int days)
    {
        // Arrange
        const string hotelId = "1";
        const RoomType roomType = RoomType.DBL;

        var avaliableFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        var avaliableTo = DateOnly.FromDateTime(DateTime.Now.AddDays(days));

        var hotel = AddDefaultHotelToRepository(hotelId);
        _bookingRepositoryMock.Setup(x => x.GetAll(It.IsAny<Func<Booking, bool>>())).Returns(new List<Booking>());

        // Act
        var result = await _handler.Handle(new SearchQuery(hotelId, days, roomType), default);

        // Assert
        result.Should().NotBeNull();
        result.RoomsAvailability.Should().ContainSingle();
        result.RoomsAvailability.Single().Should().BeEquivalentTo(new SearchQueryResponseDetails(avaliableFrom, avaliableTo, 2));
    }

    [Fact]
    public async Task HandleQueryWhenRoomHasACoupleOfBookingsThenShouldReturnAdjustedAvailableList()
    {
        // Arrange
        const string hotelId = "1";
        const RoomType roomType = RoomType.DBL;

        var hotel = AddDefaultHotelToRepository(hotelId);
        var bookings = BookingBuilder.BuildAsList(
          BookingBuilder.Init()
              .WithHotelId(hotelId)
              .WithArrival(DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
              .WithDeparture(DateOnly.FromDateTime(DateTime.Now.AddDays(3)))
              .WithRoomType(RoomType)
              .Build(),
          BookingBuilder.Init()
              .WithHotelId(hotelId)
              .WithArrival(DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
              .WithDeparture(DateOnly.FromDateTime(DateTime.Now.AddDays(3)))
              .WithRoomType(RoomType)
              .Build(),
           BookingBuilder.Init()
              .WithHotelId(hotelId)
              .WithArrival(DateOnly.FromDateTime(DateTime.Now.AddDays(6)))
              .WithDeparture(DateOnly.FromDateTime(DateTime.Now.AddDays(6)))
              .WithRoomType(RoomType)
              .Build()
          );

        _bookingRepositoryMock.Setup(x => x.GetAll(It.IsAny<Func<Booking, bool>>())).Returns(bookings);

        // Act
        var result = await _handler.Handle(new SearchQuery(hotelId, 10, roomType), default);

        // Assert
        result.Should().NotBeNull();
        result.RoomsAvailability.Should().HaveCount(4);

        var firstPart = result.RoomsAvailability.First();
        firstPart.Should().BeEquivalentTo(new SearchQueryResponseDetails(DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), 2));

        var secondPart = result.RoomsAvailability.Skip(1).First();
        secondPart.Should().BeEquivalentTo(new SearchQueryResponseDetails(DateOnly.FromDateTime(DateTime.Now.AddDays(4)),
            DateOnly.FromDateTime(DateTime.Now.AddDays(5)), 2));

        var thirdPart = result.RoomsAvailability.Skip(2).First();
        thirdPart.Should().BeEquivalentTo(new SearchQueryResponseDetails(DateOnly.FromDateTime(DateTime.Now.AddDays(6)),
            DateOnly.FromDateTime(DateTime.Now.AddDays(6)), 1));

        var fourthPart = result.RoomsAvailability.Last();
        fourthPart.Should().BeEquivalentTo(new SearchQueryResponseDetails(DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
            DateOnly.FromDateTime(DateTime.Now.AddDays(10)), 2));
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
