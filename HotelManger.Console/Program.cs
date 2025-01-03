using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HotelManger.ConsoleApp
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            // Create the Host
            var host = CreateHostBuilder(args).Build();

            // Run the application
            var command = await Init(args, host.Services);
            return command;
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Register your services here
                    // services.AddSingleton<IHotelBookingService, HotelBookingService>();
                    // Add logging service
                    services.AddLogging(configure => configure.AddConsole());
                });

        private static async Task<int> Init(string[] args, IServiceProvider services)
        {
            var hotelsOption = new Option<FileInfo>(
                "--hotels",
                "The path to the hotels JSON file")
            {
                IsRequired = true // Make the option mandatory
            };

            var bookingsOption = new Option<FileInfo>(
                "--bookings",
                "The path to the bookings JSON file")
            {
                IsRequired = true // Make the option mandatory
            };

            // Create the root command
            var rootCommand = new RootCommand
            {
                hotelsOption,
                bookingsOption
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
}
