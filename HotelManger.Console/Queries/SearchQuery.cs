using HotelManager.Shared;
using HotelManager.Shared.Factories;
using HotelManger.ConsoleApp.Extensions;
using MediatR;
using System.Text;

namespace HotelManger.ConsoleApp.Queries;

internal static class SearchQuery
{
    public static async Task RunQuery(string userInput, IMediator mediator)
    {
        var query = SearchQueryFactory.Create(userInput.GetQueryInputs(AvailableQueries.Search));
        var result = await mediator.Send(query);

        if (result.RoomsAvailability.Count == 0)
        {
            Console.WriteLine("");
            return;
        }

        var sb = new StringBuilder();
        foreach (var availability in result.RoomsAvailability)
        {
            if (availability.AvailableFrom == availability.AvailableTo)
            {
                sb.AppendLine($"({availability.AvailableFrom.ConvertDateToString()}, {availability.RoomCount}), ");
            }
            else
            {
                sb.AppendLine($"({availability.AvailableFrom.ConvertDateToString()}-{availability.AvailableTo.ConvertDateToString()}, {availability.RoomCount}), ");
            }
        }

        var message = sb.ToString();
        Console.WriteLine(message[..^2]);
    }
}