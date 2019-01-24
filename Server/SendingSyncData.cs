using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketSample.Server
{
    class SendingSyncData : ISendingData
    {
        public IEnumerable<Player> MovedPlayers { get; }

        public SendingSyncData(IEnumerable<Player> movedPlayers)
        {
            this.MovedPlayers = movedPlayers;
        }
    }
}
