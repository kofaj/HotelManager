using HotelManager.Shared.Domain;
using HotelManager.Shared.Repositories;
using HotelManager.Shared.Services.JsonConverters;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HotelManager.Shared.Services;

public class AddBookingsAndHotelsToRepositoriesFacade
{
    private readonly IInMemoryRepository<Booking> _bookingsRepository;
    private readonly IInMemoryRepository<Hotel> _hotelsRepository;
    private readonly ILogger<AddBookingsAndHotelsToRepositoriesFacade> _logger;

    public AddBookingsAndHotelsToRepositoriesFacade(IInMemoryRepository<Booking> bookingsRepository, IInMemoryRepository<Hotel> hotelsRepository,
        ILogger<AddBookingsAndHotelsToRepositoriesFacade> logger)
    {
        _bookingsRepository = bookingsRepository;
        _hotelsRepository = hotelsRepository;
        _logger = logger;
    }

    public void AddBookingsAndHotelsToRepositories(string rawHotels, string rawBookings)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        jsonOptions.Converters.Add(new DateOnlyConverter());

        try
        {
            var bookings = JsonSerializer.Deserialize<List<Booking>>(rawBookings, jsonOptions);
            var hotels = JsonSerializer.Deserialize<List<Hotel>>(rawHotels, jsonOptions);

            if (bookings?.Any() != true || hotels?.Any() != true)
            {
                throw new InvalidOperationException("Error: Deserialization failed.");
            }

            _bookingsRepository.AddRange(bookings);
            _hotelsRepository.AddRange(hotels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error: Deserialization failed.");
            throw;
        }
    }
}