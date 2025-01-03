using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using HotelManager.Shared.Extensions;

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
        var helpOption = new Option<bool>("--help", "Show command line help");

        // Create the root command
        var rootCommand = new RootCommand
            {
                hotelsOption,
                bookingsOption,
                helpOption
            };

        rootCommand.Description = "MyApp: A tool for handling hotels and bookings.";

        rootCommand.Handler = CommandHandler.Create<FileInfo, FileInfo>((hotels, bookings) =>
        {
            // Resolve the service using Dependency Injection
            var logger = services.GetRequiredService<ILogger<Program>>();
            HandleCommand(hotels, bookings, logger);
        });

        // Invoke the root command with the given args
        return await rootCommand.InvokeAsync(args);
    }

    static void HandleCommand(FileInfo hotelsFile, FileInfo bookingsFile, ILogger logger)
    {
        logger.LogInformation($"Hotels file: {hotelsFile.FullName}");
        logger.LogInformation($"Bookings file: {bookingsFile.FullName}");

        // Load and process the files here
        if (!hotelsFile.Exists || !bookingsFile.Exists)
        {
            logger.LogError("Error: One or more files do not exist.");
            return;
        }

        logger.LogInformation("Processing files...");
        // Example of loading file contents
        string hotelsContent = File.ReadAllText(hotelsFile.FullName);
        string bookingsContent = File.ReadAllText(bookingsFile.FullName);

        logger.LogInformation("Hotels data:");
        logger.LogInformation(hotelsContent);

        logger.LogInformation("Bookings data:");
        logger.LogInformation(bookingsContent);
    }
}
