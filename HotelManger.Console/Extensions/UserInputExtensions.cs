namespace HotelManger.ConsoleApp.Extensions;

internal static class UserInputExtensions
{
    public static string GetQueryInputs(this string userInput, string queryName)
    {
        var queryInput = userInput.Replace(queryName, "").Replace("(", "").Replace(")", "");
        return queryInput;
    }

    public static string ConvertDateToString(this DateOnly date)
    {
        return date.ToString("yyyyMMdd");
    }
}
