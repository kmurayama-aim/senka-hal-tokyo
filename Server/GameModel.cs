using System;
using System.Collections.Generic;
using System.Timers;
using Newtonsoft.Json;
using WebSocketSample.RPC;
using System.Linq;

namespace WebSocketSample.Server
{
    public class GameModel
    {
        Dictionary<int, Player> players = new Dictionary<int, Player>();
        Dictionary<int, Item> items = new Dictionary<int, Item>();

        int uidCounter;

        public event Action<string, string> sendTo;
        public event Action<string> broadcast;

        public GameModel()
        {
            StartSpawnTimer();
        }

        public void OnUpdate()
        {
            Sync();
        }

        public void OnPing(string senderId)
        {
            Console.WriteLine(">> Ping");

            var pingRpc = new Ping(new PingPayload("pong"));
            var pingJson = JsonConvert.SerializeObject(pingRpc);
            sendTo(pingJson, senderId);

            Console.WriteLine("<< Pong");
        }

        public void OnLogin(string senderId, LoginPayload loginPayload)
        {
            Console.WriteLine(">> Login");

            var player = new Player(uidCounter++, loginPayload.Name, new Position(0f, 0f, 0f), 0);
            lock (players)
            {
                players[player.Uid] = player;
            }

            var loginResponseRpc = new LoginResponse(new LoginResponsePayload(player.Uid));
            var loginResponseJson = JsonConvert.SerializeObject(loginResponseRpc);
            sendTo(loginResponseJson, senderId);

            Console.WriteLine(player.ToString() + " login.");

            Environment(senderId);
        }

        public void OnPlayerUpdate(string senderId, PlayerUpdatePayload playerUpdatePayload)
        {
            Console.WriteLine(">> PlayerUpdate");

            Player player;
            if (players.TryGetValue(playerUpdatePayload.Id, out player))
            {
                player.SetPosition(playerUpdatePayload.Position);
            }
        }

        public void OnGetItem(string senderId, GetItemPayload getItemPayload)
        {
            Console.WriteLine(">> GetItem");

            var itemId = getItemPayload.ItemId;
            if (items.ContainsKey(itemId))
            {
                if (items[itemId].Position.Y != getItemPayload.ItemPosition.Y
                    || items[itemId].Position.Z != getItemPayload.ItemPosition.Z)
                {
                    var itemSpawnPos = items[getItemPayload.ItemId].Position;
                    var itemOnGetPos = getItemPayload.ItemPosition;
                    var itemMoveDistance = new Position(itemSpawnPos.X - itemOnGetPos.X, itemSpawnPos.Y - itemOnGetPos.Y, itemSpawnPos.Z - itemOnGetPos.Z);
                    Console.WriteLine("Cheat!! PlayerId:{0}, ItemMoveDistance:({1}, {2}, {3})",
                        getItemPayload.PlayerId, itemMoveDistance.X, itemMoveDistance.Y,itemMoveDistance.Z);
                    return;
                }

                players[getItemPayload.PlayerId].Score += GetItemScore(items[itemId].Type);
                items.Remove(itemId);

                var deleteItemRpc = new DeleteItem(new DeleteItemPayload(itemId));
                var deleteItemJson = JsonConvert.SerializeObject(deleteItemRpc);
                broadcast(deleteItemJson);
            }
            else
            {
                Console.WriteLine("Not found ItemId: "+ itemId);
            }
        }
        int GetItemScore(ItemType type)
        {
            var score = 0;
            switch (type)
            {
                case ItemType.Normal:
                    score = 1;
                    break;
                case ItemType.Rare:
                    score = 3;
                    break;
            }
            return score;
        }

        public void OnCollision(string senderId, CollisionPayload payload)
        {
            if (!players.ContainsKey(payload.AlphaId)) { return; }
            if (!players.ContainsKey(payload.BravoId)) { return; }

            var alphaPlayer = players[payload.AlphaId];
            var bravoPlayer = players[payload.BravoId];

            if (alphaPlayer.Score == bravoPlayer.Score) { return; }

            var loser = alphaPlayer.Score < bravoPlayer.Score ? alphaPlayer : bravoPlayer;

            lock (players)
            {
                players.Remove(loser.Uid);
            }

            var deletePlayerRpc = new DeletePlayer(new DeletePlayerPayload(loser.Uid));
            var deletePlayerJson = JsonConvert.SerializeObject(deletePlayerRpc);
            broadcast(deletePlayerJson);
        }

        void Sync()
        {
            var movedPlayers = new List<Player>();
            lock (players)
            {
                movedPlayers = players.Values.Where(player => player.isPositionChanged).ToList();
                foreach (var player in movedPlayers)
                {
                    player.isPositionChanged = false;
                }
            }

            if (movedPlayers.Count == 0) return;

            var movedRpcPlayers = movedPlayers.Select(player => new RPC.Player(player.Uid, player.Position, player.Score)).ToList();
            var syncRpc = new Sync(new SyncPayload(movedRpcPlayers));
            var syncJson = JsonConvert.SerializeObject(syncRpc);
            broadcast(syncJson);
        }

        int itemSpawnConuter = 0;
        void StartSpawnTimer()
        {
            var random = new Random();
            var timer = new Timer(3000);
            timer.Elapsed += (_, __) =>
            {
                if (players.Count == 0) return;

                var itemType = new RPC.ItemType();
                itemSpawnConuter++;
                if (itemSpawnConuter % 3 != 0)
                    itemType = ItemType.Normal;
                else
                    itemType = ItemType.Rare;

                var randomX = random.Next(-5, 5);
                var randomZ = random.Next(-5, 5);
                var position = new Position(randomX, 0.5f, randomZ);
                var item = new Item(uidCounter++, itemType, position);
                items.Add(item.Id, item);

                var rpcItem = new RPC.Item(item.Id, itemType, item.Position);
                var spawnRpc = new Spawn(new SpawnPayload(rpcItem));
                var spawnJson = JsonConvert.SerializeObject(spawnRpc);
                broadcast(spawnJson);

                Console.WriteLine("<< Spawn");
            };
            timer.Start();
        }

        void Environment(string id)
        {
            var itemsRpc = new List<RPC.Item>();
            foreach (var item in items.Values)
            {
                var itemRpc = new RPC.Item(item.Id, item.Type, item.Position);
                itemsRpc.Add(itemRpc);
            }

            var environmentRpc = new RPC.Environment(new EnvironmentPayload(itemsRpc));
            var environmentJson = JsonConvert.SerializeObject(environmentRpc);
            sendTo(environmentJson, id);
        }
    }
}
