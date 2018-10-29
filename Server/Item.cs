using WebSocketSample.RPC;

namespace WebSocketSample.Server
{
    public class Item
    {
        public int Id;
        public ItemType Type;
        public Position Position;

        public Item(int id, ItemType type, Position position)
        {
            Id = id;
            Type = type;
            Position = position;
        }
    }
}
