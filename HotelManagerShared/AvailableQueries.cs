namespace HotelManager.Shared;

public static class AvailableQueries
{
    public static IReadOnlyCollection<string> GetAvailableCommands()
        => [Availability, Search];

    public const string Availability = "Availability";
    public const string Search = "Search";
}
