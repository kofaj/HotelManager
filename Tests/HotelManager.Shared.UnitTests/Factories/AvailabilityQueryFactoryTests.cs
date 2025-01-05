using FluentAssertions;
using HotelManager.Shared.Domain;
using HotelManager.Shared.Factories;
using System.ComponentModel.DataAnnotations;

namespace HotelManager.Shared.UnitTests.Factories;

public class AvailabilityQueryFactoryTests
{
    [Theory]
    [InlineData("H1,20240901,SGL", "H1", "2024-09-01", null, "SGL")]
    [InlineData("H1,20240901-20240903,DBL", "H1", "2024-09-01", "2024-09-03", "DBL")]
    public void Create_ValidCommand_ReturnsExpectedQuery(
        string command, string expectedHotelId, string expectedStartDate, string expectedEndDate, string expectedRoomType)
    {
        // Act
        var query = AvailabilityQueryFactory.Create(command);

        // Assert
        query.Should().NotBeNull();
        query.HotelId.Should().Be(expectedHotelId);
        query.DateRange.Should().NotBeNull();
        query.DateRange.Length.Should().Be(expectedEndDate == null ? 1 : 2);
        query.DateRange[0].ToString("yyyy-MM-dd").Should().Be(expectedStartDate);

        if (expectedEndDate != null)
        {
            query.DateRange[1].ToString("yyyy-MM-dd").Should().Be(expectedEndDate);
        }

        query.RoomType.Should().Be(Enum.Parse<RoomType>(expectedRoomType));
    }

    [Theory]
    [InlineData("H1,20240901", "Command is not recognized: H1,20240901")]
    [InlineData("H1,20240901,", "Room Type is not recognized: ")]
    [InlineData("H1,,SGL", "Date range is not recognized: ")]
    [InlineData(",20240901,SGL", "Hotel id is empty")]
    [InlineData("H1,invalidDate,SGL", "Date range is not recognized: invalidDate")]
    [InlineData("H1,20240901-invalid,SGL", "Date range is not recognized: 20240901-invalid")]
    [InlineData("H1,20240901-202409,SGL", "Date range is not recognized: 20240901-202409")]
    public void Create_InvalidCommand_ThrowsArgumentException(string command, string expectedErrorMessage)
    {
        // Act
        Action act = () => AvailabilityQueryFactory.Create(command);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage(expectedErrorMessage);
    }

    [Theory]
    [InlineData("H1,20240901-20240903,INVALID")]
    public void Create_InvalidRoomType_ThrowsValidationException(string command)
    {
        // Act
        Action act = () => AvailabilityQueryFactory.Create(command);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Room type is not recognized: INVALID");
    }
}
