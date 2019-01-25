using System;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using WebSocketSample.RPC;
using System.Collections.Generic;
using System.Linq;

namespace WebSocketSample.Server
{
    class SocketService : WebSocketBehavior
    {
        IMessenger messenger;
        WebSocketServer webSocketServer;
        string serviceName;

        public SocketService(IMessenger messenger, WebSocketServer webSocketServer, string serviceName)
        {
            this.messenger = messenger;
            this.webSocketServer = webSocketServer;
            this.serviceName = serviceName;

            messenger.OnSendTo += SendTo;
            messenger.OnBroadCast += Broadcast;
        }

        protected override void OnOpen()
        {
            messenger.OnOpen();
        }

        protected override void OnClose(CloseEventArgs e)
        {
            messenger.OnClose();
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            var header = JsonConvert.DeserializeObject<Header>(e.Data).Method;
            //本当はe.Dataをデシリアライズして送りたい
            var remoteMessage = new RemoteMessage(ID, header, e.Data);
            messenger.OnMessage(remoteMessage);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            var remoteError = new RemoteError(e.Exception, e.Message);
            messenger.OnError(remoteError);
        }

        public void SendTo(ISendingData data, string id)
        {
            var message = FormatWebSocketMessage(data);

            webSocketServer.WebSocketServices[serviceName].Sessions.SendTo(message, id);
            Console.WriteLine("<< SendTo: " + id + " " + message);
        }

        public void Broadcast(ISendingData data)
        {
            var message = FormatWebSocketMessage(data);

            webSocketServer.WebSocketServices[serviceName].Sessions.Broadcast(message);
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
