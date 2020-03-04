using Abstractions;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logger
{
    public class InMemoryLogger : ILogger
    {
        private List<string> Logs { get; }

        public InMemoryLogger()
        {
            Logs = new List<string>();
        }

        public event EventHandler OnLog;

        public void Log(string message)
        {
            var entry = $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()} : {message}";
            if (Logs.Count > 99)
            {
                Logs.RemoveRange(0, 1);
            }
            Logs.Add(entry);
            OnLog?.Invoke(this, new LogEventArgs { NewMessage = entry });
        }

        public IEnumerable<string> GetAll()
            => new List<string>(Logs);

        public IEnumerable<string> GetLatest(int count)
            => new List<string>(Logs.TakeLast(count));
    }
}
