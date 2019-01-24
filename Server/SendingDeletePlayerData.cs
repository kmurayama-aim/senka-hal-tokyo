using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketSample.Server
{
    class SendingDeletePlayerData
    {
        public int DeletingPlayerUid { get; }

        public SendingDeletePlayerData(int deletingPlayerUid)
        {
            this.DeletingPlayerUid = deletingPlayerUid;
        }
    }
}
