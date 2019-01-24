using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketSample.Server
{
    class SendingDeleteItemData : ISendingData
    {
        public int DeletingItemId { get; }

        public SendingDeleteItemData(int deletingItemId)
        {
            this.DeletingItemId = deletingItemId;
        }
    }
}
