using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace EMG.Extensions.Logging.Loggly
{
    public interface ILogglyProcessor : IDisposable
    {
        void EnqueueMessage(LogglyMessage message);
    }

    public class LogglyProcessor : ILogglyProcessor
    {
        private readonly ILogglyClient _client;
        private readonly ISubject<LogglyMessage> _messageSubject = new Subject<LogglyMessage>();
        private readonly IDisposable _subscription;

        public LogglyProcessor(ILogglyClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _subscription = _messageSubject.Buffer(TimeSpan.FromMilliseconds(50)).Subscribe(ProcessLogQueue);
        }

        public void EnqueueMessage(LogglyMessage message)
        {
            _messageSubject.OnNext(message);
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }

        private async void ProcessLogQueue(IList<LogglyMessage> items)
        {
            if (items.Count > 0)
            {
                await _client.PublishManyAsync(items);
            }
        }
    }
}
