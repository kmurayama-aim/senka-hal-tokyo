
namespace WebSocketSample.Server
{
    class Item
    {
        public int Id;
        public PositionData Position;

        public Item(int id, PositionData position)
        {
            Id = id;
            Position = position;
        }
    }
}
