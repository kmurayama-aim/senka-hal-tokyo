using System;
using System.Collections.Generic;
using System.Timers;

namespace WebSocketSample.Server
{
    class GameModel
    {
        Dictionary<int, Player> players = new Dictionary<int, Player>();
        Dictionary<int, Item> items = new Dictionary<int, Item>();

        int uidCounter;

        public event Action<ISendingData, string> sendTo;
        public event Action<ISendingData> broadcast;

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

            var pongData = new SendingPongData("pong");
            sendTo(pongData, senderId);

            Console.WriteLine("<< Pong");
        }

        public void OnLogin(string senderId, ReceivedLoginData loginMessage)
        {
            Console.WriteLine(">> Login");

            var player = new Player(uidCounter++, loginMessage.Name, new PositionData(0f, 0f, 0f), 0);
            lock (players)
            {
                players[player.Uid] = player;
            }

            var loginResponseData = new SendingLoginResponseData(player.Uid);
            sendTo(loginResponseData, senderId);

            Console.WriteLine(player.ToString() + " login.");

            Environment(senderId);
        }

        public void OnPlayerUpdate(string senderId, ReceivedPlayerUpdateData playerUpdateMessage)
        {
            Console.WriteLine(">> PlayerUpdate");

            Player player;
            if (players.TryGetValue(playerUpdateMessage.Id, out player))
            {
                player.SetPosition(playerUpdateMessage.Position);
            }
        }

        public void OnGetItem(string senderId, ReceivedGetItemData getItemPayload)
        {
            Console.WriteLine(">> GetItem");

            var itemId = getItemPayload.ItemId;
            if (items.ContainsKey(itemId))
            {
                items.Remove(itemId);
                players[getItemPayload.PlayerId].Score++;

                var deleteItemData = new SendingDeleteItemData(itemId);
                broadcast(deleteItemData);
            }
            else
            {
                Console.WriteLine("Not found ItemId: "+ itemId);
            }
        }

        public void OnCollision(string senderId, ReceivedCollisionData payload)
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

            var deletePlayerData = new SendingDeletePlayerData(loser.Uid);
            broadcast(deletePlayerData);
        }

        void Sync()
        {
            if (players.Count == 0) return;

            var movedPlayers = new List<Player>();
            lock (players)
            {
                foreach (var player in players.Values)
                {
                    if (!player.isPositionChanged) continue;

                    movedPlayers.Add(player);
                    player.isPositionChanged = false;
                }
            }

            if (movedPlayers.Count == 0) return;

            var syncData = new SendingSyncData(movedPlayers);
            broadcast(syncData);
        }

        void StartSpawnTimer()
        {
            var random = new Random();
            var timer = new Timer(3000);
            timer.Elapsed += (_, __) =>
            {
                if (players.Count == 0) return;

                var randomX = random.Next(-5, 5);
                var randomZ = random.Next(-5, 5);
                var position = new PositionData(randomX, 0.5f, randomZ);
                var item = new Item(uidCounter++, position);
                items.Add(item.Id, item);

                var spawnData = new SendingSpawnData(item);
                broadcast(spawnData);

                Console.WriteLine("<< Spawn");
            };
            timer.Start();
        }

        void Environment(string id)
        {
            var environmentData = new SendingEnvironmentData(items.Values);
            sendTo(environmentData, id);
        }
    }
}
