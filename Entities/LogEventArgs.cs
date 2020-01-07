using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class LogEventArgs : EventArgs
    {
        public string NewMessage { get; set; }
    }
}
