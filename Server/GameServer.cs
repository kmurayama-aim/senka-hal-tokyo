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
        const ConsoleKey EXIT_KEY = ConsoleKey.Q;

        GameModel model;
        GameService service;

        WebSocketServer webSocketServer;

        public GameServer(GameModel model, GameService service, WebSocketServer webSocketServer)
        {
            this.service = service;
            this.model = model;
            this.webSocketServer = webSocketServer;

            model.sendTo += service.SendTo;
            model.broadcast += service.Broadcast;

            service.OnPing += model.OnPing;
            service.OnLogin += model.OnLogin;
            service.OnPlayerUpdate += model.OnPlayerUpdate;
            service.OnGetItem += model.OnGetItem;
            service.OnCollision += model.OnCollision;

            webSocketServer.AddWebSocketService<SocketService>(service.ServiceName, () =>
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
    }
}