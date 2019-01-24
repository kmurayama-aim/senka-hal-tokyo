using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketSample.Server
{
    class SendingEnvironmentData
    {
        public IEnumerable<Item> AllItems { get; }

        public SendingEnvironmentData(IEnumerable<Item> allItems)
        {
            this.AllItems = allItems;
        }
    }
}
