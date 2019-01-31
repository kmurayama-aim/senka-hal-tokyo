using System;
using System.Collections.Generic;
using System.Net;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using WebSocketSample.RPC;
using System.Linq;

namespace WebSocketSample.Server
{
    class MainClass
    {
        static void Main()
        {
            var ipv4 = Dns.GetHostAddresses("").FirstOrDefault(ipAdddress => ipAdddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            if (ipv4 == null)
                return;

            var port = 5678;
            var address = string.Format("ws://{0}:{1}", ipv4.ToString(), port);
            Console.WriteLine(address);

            var gameServer = new GameServer(address);
            gameServer.RunForever();
        }
    }
}
