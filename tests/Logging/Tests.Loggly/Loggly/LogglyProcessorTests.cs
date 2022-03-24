using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Extensions.Logging.Loggly;
using Moq;
using NUnit.Framework;

namespace Tests.Loggly
{
    [TestFixture]
    public class LogglyProcessorTests
    {
        [Test, AutoMoqData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(LogglyProcessor).GetConstructors());
        }

        [Test, AutoMoqData]
        public async Task Message_is_published_after_adding_to_queue_with_delay([Frozen] ILogglyClient client, LogglyProcessor sut, LogglyMessage message)
        {
            sut.EnqueueMessage(message);

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Mock.Get(client).Verify(p => p.PublishManyAsync(It.Is<IEnumerable<LogglyMessage>>(m => m.Contains(message))));
        }

        [Test, AutoMoqData]
        public void Message_is_not_published_after_adding_to_queue_when_disposed([Frozen] ILogglyClient client, LogglyProcessor sut, LogglyMessage message)
        {
            sut.Dispose();

            sut.EnqueueMessage(message);

            Mock.Get(client).Verify(p => p.PublishAsync(message), Times.Never);
        }

        [Test, AutoMoqData]
        public async Task Message_is_not_published_after_adding_to_queue_without_delay([Frozen] ILogglyClient client, LogglyProcessor sut, LogglyMessage message)
        {
            sut.EnqueueMessage(message);

            Mock.Get(client).Verify(p => p.PublishManyAsync(It.Is<IEnumerable<LogglyMessage>>(m => m.Contains(message))), Times.Never);
        }

        [Test, AutoMoqData]
        public async Task Message_is_published_after_adding_to_queue_without_delay_after_flush([Frozen] ILogglyClient client, LogglyProcessor sut, LogglyMessage message)
        {
            sut.EnqueueMessage(message);

            sut.FlushMessages();

            Mock.Get(client).Verify(p => p.PublishManyAsync(It.Is<IEnumerable<LogglyMessage>>(m => m.Contains(message))), Times.Once);
        }
    }
}