using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketSample.Server
{
    class ReceivedPlayerUpdateData
    {
        public int Id;
        public PositionData Position;

        public ReceivedPlayerUpdateData(int id, PositionData position)
        {
            this.Id = id;
            this.Position = position;
        }
    }
}
