using System;
using System.Collections.Generic;

namespace Abstractions
{
    public interface ILogger
    {
        event EventHandler OnLog;

        IEnumerable<string> GetAll();
        IEnumerable<string> GetLatest(int count);
        void Log(string message);
    }
}