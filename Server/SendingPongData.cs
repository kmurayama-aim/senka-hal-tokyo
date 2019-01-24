using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketSample.Server
{
    class SendingPongData
    {
        public string PongMessage { get; }

        public SendingPongData(string pongMessage)
        {
            this.PongMessage = pongMessage;
        }
    }
}
