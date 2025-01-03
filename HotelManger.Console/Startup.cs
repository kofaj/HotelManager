using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using HotelManager.Shared.Extensions;
using HotelManager.Shared.Domain;
using HotelManager.Shared.Repositories;
using HotelManager.Shared.Services;

namespace HotelManger.ConsoleApp;

internal static class Startup
{
    internal static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureServices((context, services) =>
               {
                   // Register your services here
                   // services.AddSingleton<IHotelBookingService, HotelBookingService>();
                   // Add logging service
                   services.AddLogging(configure => configure.AddConsole());
                   services.RegisterSharedServices();
               });

    internal static async Task<int> Run(string[] args, IServiceProvider services)
    {
        var hotelsOption = new Option<FileInfo>("--hotels", "The path to the hotels JSON file");
        var bookingsOption = new Option<FileInfo>("--bookings", "The path to the bookings JSON file");
        // TODO fix above option

        // Create the root command
        var rootCommand = new RootCommand
            {
                hotelsOption,
                bookingsOption,
            };

        rootCommand.Description = "MyApp: A tool for handling hotels and bookings.";

        rootCommand.Handler = CommandHandler.Create<FileInfo, FileInfo>(async (hotels, bookings) =>
        {
            var facade = services.GetRequiredService<AddBookingsAndHotelsToRepositoriesFacade>();

            await GetRequiredDataFromFiles(hotels, bookings, facade);

            Console.ReadKey();
        });

        // Invoke the root command with the given args
        return await rootCommand.InvokeAsync(args);
    }

    static async Task GetRequiredDataFromFiles(FileInfo hotelsFile, FileInfo bookingsFile, AddBookingsAndHotelsToRepositoriesFacade facade)
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
}
