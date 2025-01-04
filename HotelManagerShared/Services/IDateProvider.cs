namespace HotelManager.Shared.Services;

public interface IDateProvider
{
    DateOnly Today { get; }
}

internal class DateProvider : IDateProvider
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Now);
}
