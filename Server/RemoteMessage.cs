using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketSample.Server
{
    class RemoteMessage
    {
        public string SenderID { get; }
        public string Header { get; }
        public string Message { get; }

        public RemoteMessage(string senderID, string header, string message)
        {
            this.SenderID = senderID;
            this.Header = header;
            this.Message = message;
        }
    }
}
