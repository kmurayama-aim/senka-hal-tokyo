using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketSample.Server
{
    class SendingEnvironmentData : ISendingData
    {
        public IEnumerable<Item> AllItems { get; }

        public SendingEnvironmentData(IEnumerable<Item> allItems)
        {
            this.AllItems = allItems;
        }
    }
}
