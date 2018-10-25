using WebSocketSample.RPC;

namespace WebSocketSample.Server
{
    class Player
    {
        public readonly int Uid;
        public readonly string Name;
        public Position Position;
        public LocalScale LocalScale;
        public int Score;
        public bool isPositionChanged;
        public bool isLocalScaleChanged;

        public Player(int uid, string name, Position position, LocalScale localScale, int score)
        {
            Uid = uid;
            Name = name;
            Position = position;
            LocalScale = localScale;
            Score = score;
        }

        public void SetPosition(Position position)
        {
            if (Position.X != position.X || Position.Y != position.Y || Position.Z != position.Z)
            {
                Position = position;
                isPositionChanged = true;
            }
        }
        public void SetLocalScale(LocalScale localScale) {
            LocalScale = localScale;
            isLocalScaleChanged = true;
        }
        public override string ToString()
        {
            return string.Format(
                "<Player(uid={0}, name={1}, x={2}, y={3}, z={4}, score={5})>",
                Uid,
                Name,
                Position.X, Position.Y, Position.Z,
                Score
            );
        }
    }
}