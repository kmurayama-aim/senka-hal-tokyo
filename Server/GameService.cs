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
        public event Action<string, LoginPayload> OnLogin;
        public event Action<string, PlayerUpdatePayload> OnPlayerUpdate;
        public event Action<string, GetItemPayload> OnGetItem;
        public event Action<string, CollisionPayload> OnCollision;

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
                        OnLogin(e.SenderID, loginPayload);
                        break;
                    }
                case "player_update":
                    {
                        var playerUpdatePayload = JsonConvert.DeserializeObject<PlayerUpdate>(e.Message).Payload;
                        OnPlayerUpdate(e.SenderID, playerUpdatePayload);
                        break;
                    }
                case "get_item":
                    {
                        var getItemPayload = JsonConvert.DeserializeObject<GetItem>(e.Message).Payload;
                        OnGetItem(e.SenderID, getItemPayload);
                        break;
                    }
                case "collision":
                    {
                        var collisionPayload = JsonConvert.DeserializeObject<RPC.Collision>(e.Message).Payload;
                        OnCollision(e.SenderID, collisionPayload);
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