namespace HotelManager.Shared.Commands;

public static class AvailableCommands
{
    public static IReadOnlyCollection<string> GetAvailableCommands()
        => [Availability, Search];

    public const string Availability = "Availability";
    public const string Search = "Search";
}
