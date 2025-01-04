using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using HotelManager.Shared.Extensions;
using HotelManager.Shared.Services;
using HotelManager.Shared.Commands;
using MediatR;
using HotelManager.Shared.Factories;

namespace HotelManger.ConsoleApp;

internal static class Startup
{
    internal static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureServices((context, services) =>
               {
                   services.AddLogging(configure => configure.AddConsole());
                   services.RegisterSharedServices();
               });

    internal static async Task<int> Run(string[] args, IServiceProvider services)
    {
        var hotelsOption = new Option<FileInfo>("--hotels", "The path to the hotels JSON file");
        var bookingsOption = new Option<FileInfo>("--bookings", "The path to the bookings JSON file");

        var rootCommand = new RootCommand
            {
                hotelsOption,
                bookingsOption,
            };

        rootCommand.Description = "MyApp: A tool for handling hotels and bookings.";

        rootCommand.Handler = CommandHandler.Create<FileInfo, FileInfo>(async (hotels, bookings) =>
        {
            var facade = services.GetRequiredService<AddBookingsAndHotelsToRepositoriesFacade>();
            var mediator = services.GetRequiredService<IMediator>();

            await GetRequiredDataFromFiles(hotels, bookings, facade);

            await ProcessUserCommands(mediator);
        });

        // Invoke the root command with the given args
        return await rootCommand.InvokeAsync(args);
    }

    private static async Task GetRequiredDataFromFiles(FileInfo hotelsFile, FileInfo bookingsFile, AddBookingsAndHotelsToRepositoriesFacade facade)
    {
        if (!hotelsFile.Exists || !bookingsFile.Exists)
        {
            Console.WriteLine("Error: One or more files do not exist.");
            return;
        }

        // In real-case scenarios I would add CT to the following calls
        string hotelsContent = await File.ReadAllTextAsync(hotelsFile.FullName);
        string bookingsContent = await File.ReadAllTextAsync(bookingsFile.FullName);

        facade.AddBookingsAndHotelsToRepositories(hotelsContent, bookingsContent);
    }

    private static async ValueTask ProcessUserCommands(IMediator mediator)
    {
        Console.WriteLine("Please enter a command or write help to get a list of available commands.");

        while (true)
        {
            var userCommand = Console.ReadLine();

            if (userCommand == null)
            {
                Environment.Exit(0);
            }

            if (userCommand == "help")
            {
                Console.WriteLine($"Available commands: {string.Join(',', AvailableCommands.GetAvailableCommands())}");
            }

            if (userCommand.StartsWith(AvailableCommands.Availability))
            {
                var command = AvailabilityCommandFactory.Create(userCommand);
                var result = await mediator.Send(command);
            }

            if (userCommand == AvailableCommands.Search.ToLower())
            {

            }
        }
    }
}
