using FluentAssertions;
using HotelManager.Shared.Domain;
using HotelManager.Shared.Factories;

namespace HotelManager.Shared.UnitTests.Factories;

public class SearchQueryFactoryTests
{
    [Theory]
    [InlineData("H1,365,SGL", "H1", 365, "SGL")]
    [InlineData("H2,7,DBL", "H2", 7, "DBL")]
    public void Create_ValidCommand_ReturnsExpectedQuery(string command, string expectedHotelId, int expectedDaysCount, string expectedRoomType)
    {
        // Act
        var query = SearchQueryFactory.Create(command);

        // Assert
        query.Should().NotBeNull();
        query.HotelId.Should().Be(expectedHotelId);
        query.Days.Should().Be(expectedDaysCount);
        query.RoomType.Should().Be(Enum.Parse<RoomType>(expectedRoomType));
    }

    [Theory]
    [InlineData("H1,365", "Command is not recognized: H1,365")]
    [InlineData("H1,,SGL", "Days count is not recognized: ")]
    [InlineData(",365,SGL", "Hotel id is empty")]
    [InlineData("H1,invalid,SGL", "Days count is not recognized: invalid")]
    public void Create_InvalidCommand_ThrowsValidationException(string command, string expectedErrorMessage)
    {
        // Act
        Action act = () => SearchQueryFactory.Create(command);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage(expectedErrorMessage);
    }

    [Fact]
    public void Create_InvalidRoomType_ThrowsValidationException()
    {
        // Arrange
        var command = "H1,365,INVALID";

        // Act
        Action act = () => SearchQueryFactory.Create(command);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Room type is not recognized: INVALID");
    }

    [Theory]
    [InlineData("H1,365, SGL", "H1", 365, "SGL")]
    [InlineData("H1 , 365 , SGL", "H1", 365, "SGL")]
    public void Create_ValidCommandWithSpaces_ParsesCorrectly(string command, string expectedHotelId, int expectedDaysCount, string expectedRoomType)
    {
        // Act
        var query = SearchQueryFactory.Create(command);

        // Assert
        query.Should().NotBeNull();
        query.HotelId.Should().Be(expectedHotelId);
        query.Days.Should().Be(expectedDaysCount);
        query.RoomType.Should().Be(Enum.Parse<RoomType>(expectedRoomType));
    }
}
