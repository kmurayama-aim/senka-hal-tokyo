using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketSample.Server
{
    class ReceivedGetItemData
    {
        public int ItemId;
        public int PlayerId;

        public ReceivedGetItemData(int itemId, int playerId)
        {
            this.ItemId = itemId;
            this.PlayerId = playerId;
        }
    }
}
