using HotelManager.Shared;
using HotelManager.Shared.Factories;
using HotelManger.ConsoleApp.Extensions;
using MediatR;
using System.Text;

namespace HotelManger.ConsoleApp.Queries;

internal class SearchQuery
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
            if (availability.AvaliableFrom == availability.AvailableTo)
            {
                sb.AppendLine($"({availability.AvaliableFrom.ConvertDateToString()}, {availability.RoomCount}), ");
            }
            else
            {
                sb.AppendLine($"({availability.AvaliableFrom.ConvertDateToString()}-{availability.AvailableTo.ConvertDateToString()}, {availability.RoomCount}), ");
            }
        }

        Console.WriteLine(sb.ToString().TrimEnd(new char[] { ',', ' ' }));
    }
}
