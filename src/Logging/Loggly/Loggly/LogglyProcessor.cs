using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace EMG.Extensions.Logging.Loggly
{
    public interface ILogglyProcessor : IDisposable
    {
        void EnqueueMessage(LogglyMessage message);

        void FlushMessages();
    }

    public class LogglyProcessor : ILogglyProcessor
    {
        private readonly ILogglyClient _client;
        private readonly ISubject<LogglyMessage> _messageSubject = new Subject<LogglyMessage>();
        private readonly ISubject<LogglyMessage> _flush = new Subject<LogglyMessage>();
        private readonly IDisposable _subscription;

        public LogglyProcessor(ILogglyClient client, LogglyOptions options)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _ = options ?? throw new ArgumentNullException(nameof(options));

            var closing = _messageSubject.Buffer(options.Buffer).Select(i => LogglyMessage.Default).Merge(_flush);
            _subscription = _messageSubject.Buffer(() => closing).Subscribe(ProcessLogQueue);
        }

        public void EnqueueMessage(LogglyMessage message)
        {
            _messageSubject.OnNext(message);
        }

        public void FlushMessages()
        {
           _flush.OnNext(LogglyMessage.Default);
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
