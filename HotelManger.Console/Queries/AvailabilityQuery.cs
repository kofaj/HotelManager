using HotelManager.Shared.Factories;
using HotelManger.ConsoleApp.Extensions;
using MediatR;
using HotelManager.Shared;

namespace HotelManger.ConsoleApp.Queries;

internal static class AvailabilityQuery
{
    public static async Task RunQuery(string userInput, IMediator mediator)
    {
        var query = AvailabilityQueryFactory.Create(userInput.GetQueryInputs(AvailableQueries.Availability));
        var result = await mediator.Send(query);

        Console.WriteLine($"Availability count for the specified room type and date range: {result.RoomCount}");
    }
}