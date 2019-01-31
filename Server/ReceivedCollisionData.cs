using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketSample.Server
{
    class ReceivedCollisionData
    {
        public int AlphaId;
        public int BravoId;

        public ReceivedCollisionData(int alphaId, int bravoId)
        {
            this.AlphaId = alphaId;
            this.BravoId = bravoId;
        }
    }
}
