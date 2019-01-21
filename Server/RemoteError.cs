using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketSample.Server
{
    class RemoteError
    {
        public Exception Exception { get; }
        public string Message { get; }

        public RemoteError(Exception ex, string message)
        {
            this.Exception = ex;
            this.Message = message;
        }
    }
}
