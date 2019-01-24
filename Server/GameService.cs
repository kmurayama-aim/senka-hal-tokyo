using System;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using WebSocketSample.RPC;

namespace WebSocketSample.Server
{
    class GameService : IMessageReceiver
    {
        public event Action<string> OnPing;
        public event Action<string, ReceivedLoginData> OnLogin;
        public event Action<string, ReceivedPlayerUpdateData> OnPlayerUpdate;
        public event Action<string, ReceivedGetItemData> OnGetItem;
        public event Action<string, ReceivedCollisionData> OnCollision;

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
    }
}