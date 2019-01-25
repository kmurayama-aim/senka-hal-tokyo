using System;
using WebSocketSharp.Server;
using WebSocketSample.RPC;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace WebSocketSample.Server
{
    class GameServer
    {
        const string SERVICE_NAME = "/";
        public string ServiceName { get { return SERVICE_NAME; } }
        
        const ConsoleKey EXIT_KEY = ConsoleKey.Q;

        GameModel model;
        GameService service;

        WebSocketServer webSocketServer;

        public GameServer(GameModel model, GameService service, WebSocketServer webSocketServer)
        {
            this.service = service;
            this.model = model;
            this.webSocketServer = webSocketServer;

            model.sendTo += SendTo;
            model.broadcast += Broadcast;

            service.OnPing += model.OnPing;
            service.OnLogin += model.OnLogin;
            service.OnPlayerUpdate += model.OnPlayerUpdate;
            service.OnGetItem += model.OnGetItem;
            service.OnCollision += model.OnCollision;

            webSocketServer.AddWebSocketService<SocketService>(SERVICE_NAME, () =>
            {
                return new SocketService(service);
            });
        }

        public void RunForever()
        {
            webSocketServer.Start();
            Console.WriteLine("Game Server started.");

            while (!IsInputtedExitKey())
            {
                model.OnUpdate();
            }
        }

        bool IsInputtedExitKey()
        {
            if (!Console.KeyAvailable) { return false; }

            switch (Console.ReadKey(true).Key)
            {
                default:
                    Console.WriteLine("Enter " + EXIT_KEY + " to exit the game.");
                    return false;

                case EXIT_KEY:
                    webSocketServer.Stop();
                    Console.WriteLine("Game Server terminated.");
                    return true;
            }
        }

        void SendTo(ISendingData data, string id)
        {
            var message = FormatWebSocketMessage(data);

            webSocketServer.WebSocketServices[SERVICE_NAME].Sessions.SendTo(message, id);
            Console.WriteLine("<< SendTo: " + id + " " + message);
        }

        void Broadcast(ISendingData data)
        {
            var message = FormatWebSocketMessage(data);

            webSocketServer.WebSocketServices[SERVICE_NAME].Sessions.Broadcast(message);
            Console.WriteLine("<< Broeadcast: " + message);
        }

        string FormatWebSocketMessage(ISendingData data)
        {
            string jsonMessage = string.Empty;

            switch (data)
            {
                case SendingPongData pong:
                    {
                        var pingRpc = new Ping(new PingPayload(pong.PongMessage));
                        jsonMessage = JsonConvert.SerializeObject(pingRpc);
                        break;
                    }
                case SendingLoginResponseData loginResponse:
                    {
                        var loginResponseRpc = new LoginResponse(new LoginResponsePayload(loginResponse.LoginPlayerUid));
                        jsonMessage = JsonConvert.SerializeObject(loginResponseRpc);
                        break;
                    }
                case SendingDeleteItemData deleteItem:
                    {
                        var deleteItemRpc = new DeleteItem(new DeleteItemPayload(deleteItem.DeletingItemId));
                        jsonMessage = JsonConvert.SerializeObject(deleteItemRpc);
                        break;
                    }
                case SendingDeletePlayerData deletePlayer:
                    {
                        var deletePlayerRpc = new DeletePlayer(new DeletePlayerPayload(deletePlayer.DeletingPlayerUid));
                        jsonMessage = JsonConvert.SerializeObject(deletePlayerRpc);
                        break;
                    }
                case SendingSyncData syncData:
                    {
                        var rpcPlayers = CreateRpcPlayers(syncData.MovedPlayers);
                        var syncRpc = new Sync(new SyncPayload(rpcPlayers));
                        jsonMessage = JsonConvert.SerializeObject(syncRpc);
                        break;
                    }
                case SendingSpawnData spawnData:
                    {
                        var rpcItemPos = CreateRpcPosition(spawnData.SpawnedItem.Position);
                        var rpcItem = new RPC.Item(spawnData.SpawnedItem.Id, rpcItemPos);
                        var spawnRpc = new Spawn(new SpawnPayload(rpcItem));
                        jsonMessage = JsonConvert.SerializeObject(spawnRpc);
                        break;
                    }
                case SendingEnvironmentData environmentData:
                    {
                        var rpcItems = CreateRpcItems(environmentData.AllItems);
                        var environmentRpc = new RPC.Environment(new EnvironmentPayload(rpcItems));
                        jsonMessage = JsonConvert.SerializeObject(environmentRpc);
                        break;
                    }
            }

            return jsonMessage;
        }

        //RPCに送るデータがListのため戻り値をIEnumerableにしなかった
        List<RPC.Player> CreateRpcPlayers(IEnumerable<Player> players)
        {
            return players.Select(p => CreateRpcPlayer(p)).ToList();
        }

        RPC.Player CreateRpcPlayer(Player player)
        {
            return new RPC.Player(player.Uid, CreateRpcPosition(player.Position), player.Score);
        }

        RPC.Position CreateRpcPosition(PositionData pos)
        {
            return new RPC.Position(pos.X, pos.Y, pos.Z);
        }

        //RPCに送るデータがListのため戻り値をIEnumerableにしなかった
        List<RPC.Item> CreateRpcItems(IEnumerable<Item> items)
        {
            return items.Select(item => CreateRpcItem(item)).ToList();
        }

        RPC.Item CreateRpcItem(Item item)
        {
            return new RPC.Item(item.Id, CreateRpcPosition(item.Position));
        }
    }
}