using System;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using WebSocketSample.RPC;

namespace WebSocketSample.Server
{
    class SocketService : WebSocketBehavior
    {
        GameService sendMessageTarget;

        public SocketService(GameService sendMessageTarget)
        {
            this.sendMessageTarget = sendMessageTarget;
        }

        protected override void OnOpen()
        {
        }

        protected override void OnClose(CloseEventArgs e)
        {
        }

        protected override void OnMessage(MessageEventArgs e)
        {
        }

        protected override void OnError(ErrorEventArgs e)
        {
        }
    }
}
