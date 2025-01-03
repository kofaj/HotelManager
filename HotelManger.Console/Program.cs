namespace HotelManger.ConsoleApp;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        // Create the Host
        var host = Startup.CreateHostBuilder(args).Build();

        return await Startup.Run(args, host.Services);
    }
}
