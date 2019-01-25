using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketSample.Server
{
    interface IMessenger
    {
        event Action<ISendingData, string> OnSendTo;
        event Action<ISendingData> OnBroadCast;

        void OnOpen();
        void OnClose();
        void OnMessage(RemoteMessage remoteMessage);
        void OnError(RemoteError remoteError);
    }
}
