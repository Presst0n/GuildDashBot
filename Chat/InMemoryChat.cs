using Abstractions;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chat
{
    public class InMemoryChat : IChat
    {
        private List<string> ChatMessages { get; }

        public InMemoryChat()
        {
            ChatMessages = new List<string>();
        }

        public event EventHandler OnNewMessage;

        public void CaptureMessage(string message)
        {
            var entry = $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()} : {message}";
            ChatMessages.Add(entry);
            OnNewMessage?.Invoke(this, new ChatEventArgs { NewChatMessage = entry });
        }

        public IEnumerable<string> GetAll()
            => new List<string>(ChatMessages);

        public IEnumerable<string> GetLatest(int count)
            => new List<string>(ChatMessages.TakeLast(count));
    }
}
