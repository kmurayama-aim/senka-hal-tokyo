using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketSample.Server
{
    class SendingSpawnData
    {
        public Item SpawnedItem { get; }

        public SendingSpawnData(Item spawnedItem)
        {
            this.SpawnedItem = spawnedItem;
        }
    }
}
