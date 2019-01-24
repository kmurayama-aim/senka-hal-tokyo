using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketSample.Server
{
    class SendingLoginResponseData
    {
        public int LoginPlayerUid { get; }

        public SendingLoginResponseData(int loginPlayerUid)
        {
            this.LoginPlayerUid = loginPlayerUid;
        }
    }
}
