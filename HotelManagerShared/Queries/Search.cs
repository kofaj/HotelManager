using HotelManager.Shared.Domain;
using HotelManager.Shared.Repositories;
using HotelManager.Shared.Services;
using MediatR;

namespace HotelManager.Shared.Query;

public record SearchQuery(string HotelId, int Days, RoomType RoomType) : IRequest<SearchQueryResponse>;

public record SearchQueryResponse(IReadOnlyCollection<SearchQueryResponseDetails> RoomsAvailability);

public class SearchQueryResponseDetails(DateOnly AvaliableFrom, DateOnly AvailableTo, int RoomCount)
{
    public DateOnly AvaliableFrom { get; } = AvaliableFrom;
    public DateOnly AvailableTo { get; private set; } = AvailableTo;
    public int RoomCount { get; } = RoomCount;

    public bool AddIfRequirementMet(DateOnly date, int roomsCount)
    {
        if (RoomCount == roomsCount)
        {
            AvailableTo = date;
            return true;
        }

        return false;
    }
}

// During testing I found one case, where I was not sure if it's valid or not - Is it possible to run this query with 0 days?
internal class SearchQueryHandler : IRequestHandler<SearchQuery, SearchQueryResponse>
{
    private readonly IInMemoryRepository<Hotel> _hotelRepository;
    private readonly IInMemoryRepository<Booking> _bookingRepository;
    private readonly IDateProvider _dateTimeProvider;

    public SearchQueryHandler(IInMemoryRepository<Hotel> hotelRepository, IInMemoryRepository<Booking> bookingRepository,
        IDateProvider dateTimeProvider)
    {
        _hotelRepository = hotelRepository;
        _bookingRepository = bookingRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<SearchQueryResponse> Handle(SearchQuery request, CancellationToken cancellationToken)
    {
        var hotel = _hotelRepository.GetById(request.HotelId);
        var bookings = _bookingRepository.GetAll(x => x.HotelId == request.HotelId && _dateTimeProvider.Today.AddDays(request.Days) > x.Arrival);

        var availableRooms = new List<SearchQueryResponseDetails>();

        var shouldCreateNewRecord = false;
        for (int i = 1; i <= request.Days; i++)
        {
            var date = _dateTimeProvider.Today.AddDays(i);
            var bookingsForDate = bookings.Where(x => x.Arrival <= date && x.Departure >= date).ToList();
            var roomsCount = hotel.GetAvailableRoomsCount(request.RoomType) - bookingsForDate.Count;
            if (roomsCount <= 0)
            {
                shouldCreateNewRecord = true;
                continue;
            }

            if (availableRooms.Any())
            {
                var last = availableRooms.Last();
                if (!shouldCreateNewRecord && last.AddIfRequirementMet(date, roomsCount))
                {
                    continue;
                }
                else
                {
                    availableRooms.Add(new SearchQueryResponseDetails(date, date, roomsCount));
                    shouldCreateNewRecord = false;
                }
            }
            else
            {
                availableRooms.Add(new SearchQueryResponseDetails(date, date, roomsCount));
                shouldCreateNewRecord = false;
            }
        }

        return new SearchQueryResponse(availableRooms);
    }
}
