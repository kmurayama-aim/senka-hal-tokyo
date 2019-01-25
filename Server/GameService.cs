using System;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using WebSocketSample.RPC;
using System.Collections.Generic;
using System.Linq;

namespace WebSocketSample.Server
{
    class GameService : IMessenger
    {
        const string SERVICE_NAME = "/";
        public string ServiceName { get { return SERVICE_NAME; } }
        WebSocketServer webSocketServer;

        public event Action<string> OnPing;
        public event Action<string, ReceivedLoginData> OnLogin;
        public event Action<string, ReceivedPlayerUpdateData> OnPlayerUpdate;
        public event Action<string, ReceivedGetItemData> OnGetItem;
        public event Action<string, ReceivedCollisionData> OnCollision;

        public event Action<ISendingData, string> OnSendTo;
        public event Action<ISendingData> OnBroadCast;

        public GameService(WebSocketServer webSocketServer)
        {
            this.webSocketServer = webSocketServer;
        }

        public void OnOpen()
        {
            Console.WriteLine("WebSocket opened.");
        }

        public void OnClose()
        {
            Console.WriteLine("WebSocket Close.");
        }

        public void OnMessage(RemoteMessage e)
        {
            Console.WriteLine("WebSocket Message: " + e.Message);

            switch (e.Header)
            {
                case "ping":
                    {
                        OnPing(e.SenderID);
                        break;
                    }
                case "login":
                    {
                        var loginPayload = JsonConvert.DeserializeObject<Login>(e.Message).Payload;
                        var loginMessage = new ReceivedLoginData(loginPayload.Name);
                        OnLogin(e.SenderID, loginMessage);
                        break;
                    }
                case "player_update":
                    {
                        var playerUpdatePayload = JsonConvert.DeserializeObject<PlayerUpdate>(e.Message).Payload;
                        var rpcPos = playerUpdatePayload.Position;
                        var playerUpdateMessage = new ReceivedPlayerUpdateData(playerUpdatePayload.Id, new PositionData(rpcPos.X, rpcPos.Y, rpcPos.Z));
                        OnPlayerUpdate(e.SenderID, playerUpdateMessage);
                        break;
                    }
                case "get_item":
                    {
                        var getItemPayload = JsonConvert.DeserializeObject<GetItem>(e.Message).Payload;
                        var getItemMessage = new ReceivedGetItemData(getItemPayload.ItemId, getItemPayload.PlayerId);
                        OnGetItem(e.SenderID, getItemMessage);
                        break;
                    }
                case "collision":
                    {
                        var collisionPayload = JsonConvert.DeserializeObject<RPC.Collision>(e.Message).Payload;
                        var collisionMessage = new ReceivedCollisionData(collisionPayload.AlphaId, collisionPayload.BravoId);
                        OnCollision(e.SenderID, collisionMessage);
                        break;
                    }
            }
        }

        public void OnError(RemoteError e)
        {
            Console.WriteLine("WebSocket Error: " + e);
        }

        public void SendTo(ISendingData data, string id)
        {
            var message = FormatWebSocketMessage(data);

            webSocketServer.WebSocketServices[SERVICE_NAME].Sessions.SendTo(message, id);
            Console.WriteLine("<< SendTo: " + id + " " + message);
        }

        public void Broadcast(ISendingData data)
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