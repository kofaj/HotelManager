using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace HotelManger.ConsoleApp
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            await Init(args);

            var command = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(command))
            {
                return 0;
            }




            return 0;
        }

        private static async Task<int> Init(string[] args)
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
                // Call a method to handle the command logic
                HandleCommand(hotels, bookings);
            });

            // Invoke the root command with the given args
            return await rootCommand.InvokeAsync(args);
        }

        static void HandleCommand(FileInfo hotelsFile, FileInfo bookingsFile)
        {
            Console.WriteLine($"Hotels file: {hotelsFile.FullName}");
            Console.WriteLine($"Bookings file: {bookingsFile.FullName}");

            // Load and process the files here
            if (!hotelsFile.Exists || !bookingsFile.Exists)
            {
                Console.WriteLine("Error: One or more files do not exist.");
                return;
            }

            Console.WriteLine("Processing files...");
            // Example of loading file contents
            string hotelsContent = File.ReadAllText(hotelsFile.FullName);
            string bookingsContent = File.ReadAllText(bookingsFile.FullName);

            Console.WriteLine("Hotels data:");
            Console.WriteLine(hotelsContent);

            Console.WriteLine("Bookings data:");
            Console.WriteLine(bookingsContent);
        }
    }
}
