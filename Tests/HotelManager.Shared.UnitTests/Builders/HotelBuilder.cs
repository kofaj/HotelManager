using HotelManager.Shared.Domain;

namespace HotelManager.Shared.UnitTests.Builders;

internal class HotelBuilder
{
    private string _id;
    private string _name;
    private List<RoomDetails> _roomTypes = new();
    private List<Room> _rooms = new();

    public HotelBuilder SetId(string id)
    {
        _id = id;
        return this;
    }

    public HotelBuilder SetName(string name)
    {
        _name = name;
        return this;
    }

    public HotelBuilder AddRoomDetails(RoomDetails roomDetails)
    {
        _roomTypes.Add(roomDetails);
        return this;
    }

    public HotelBuilder AddRoom(Room room)
    {
        _rooms.Add(room);
        return this;
    }

    public HotelBuilder AddRoomTypes(IEnumerable<RoomDetails> roomDetails)
    {
        _roomTypes.AddRange(roomDetails);
        return this;
    }

    public HotelBuilder AddRooms(IEnumerable<Room> rooms)
    {
        _rooms.AddRange(rooms);
        return this;
    }

    public Hotel Build()
        => new(_id, _name, _roomTypes, _rooms);

    public static HotelBuilder Init()
        => new();
}

