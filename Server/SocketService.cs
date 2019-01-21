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
            var header = JsonConvert.DeserializeObject<Header>(e.Data).Method;
            //本当はe.Dataをデシリアライズして送りたい
            var remoteMessage = new RemoteMessage(ID, header, e.Data);
            messageReceiver.OnMessage(remoteMessage);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            var remoteError = new RemoteError(e.Exception, e.Message);
            messageReceiver.OnError(remoteError);
        }
    }
}
