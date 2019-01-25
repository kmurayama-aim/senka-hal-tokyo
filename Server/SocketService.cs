using System;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using WebSocketSample.RPC;

namespace WebSocketSample.Server
{
    class SocketService : WebSocketBehavior
    {
        IMessenger messenger;

        public SocketService(IMessenger messenger)
        {
            this.messenger = messenger;
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
    }
}
