using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketSample.Server
{
    class ReceivedLoginData
    {
        public string Name { get; }

        public ReceivedLoginData(string name)
        {
            this.Name = name;
        }
    }
}
