using System;
using System.Collections.Generic;
using System.Net;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using WebSocketSample.RPC;

namespace WebSocketSample.Server
{
    class MainClass
    {
        static void Main()
        {
            IPAddress ipv4 = null;
            foreach (var ipAddress in Dns.GetHostAddresses(""))
            {
                if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipv4 = ipAddress;
                    break;
                }
            }

            var port = 5678;
            var address = string.Format("ws://{0}:{1}", ipv4.ToString(), port);
            Console.WriteLine(address);

            Initialize(address);
        }

        static void Initialize(string address)
        {
            var gameModel = new GameModel();
            var gameService = new GameService();
            var webSocketServer = new WebSocketServer(address);
            var gameServer = new GameServer(gameModel, gameService, webSocketServer);

            gameServer.RunForever();
        }
    }
}
