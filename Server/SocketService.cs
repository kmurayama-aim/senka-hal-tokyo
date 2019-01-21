using System;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using WebSocketSample.RPC;

namespace WebSocketSample.Server
{
    class SocketService : WebSocketBehavior
    {
        IMessageReceiver messageReceiver;

        public SocketService(IMessageReceiver messageReceiver)
        {
            this.messageReceiver = messageReceiver;
        }

        protected override void OnOpen()
        {
            messageReceiver.OnOpen();
        }

        protected override void OnClose(CloseEventArgs e)
        {
            messageReceiver.OnClose();
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            messageReceiver.OnMessage();
        }

        protected override void OnError(ErrorEventArgs e)
        {
            messageReceiver.OnError();
        }
    }
}
