using System;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Extensions.Logging.Loggly;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Tests.Loggly
{
    [TestFixture]
    public class LogglyLoggerTests
    {
        [Test, AutoMoqData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(LogglyLogger).GetConstructors());
        }

        [Test, AutoMoqData]
        public void Logs_from_LogglyHttpClient_are_ignored(ILogglyProcessor processor, LogglyOptions options, LogLevel level, EventId eventId, object state, Exception error, Func<object, Exception, string> formatter)
        {
            var sut = new LogglyLogger(typeof(LogglyHttpClient).FullName, processor, options);

            sut.Log(level, eventId, state, error, formatter);

            Mock.Get(processor).Verify(p => p.EnqueueMessage(It.IsAny<LogglyMessage>()), Times.Never);
        }

        [Test, AutoMoqData]
        public void Logs_are_discarded_if_not_matching_filter([Frozen] ILogglyProcessor processor, [Frozen] LogglyOptions options, LogglyLogger sut, LogLevel level, EventId eventId, object state, Exception error, Func<object, Exception, string> formatter)
        {
            options.Filter = (name, id, logLevel) => false;

            sut.Log(level, eventId, state, error, formatter);

            Mock.Get(processor).Verify(p => p.EnqueueMessage(It.IsAny<LogglyMessage>()), Times.Never);
        }

        [Test, AutoMoqData]
        public void Formatter_is_required(LogglyLogger sut, LogLevel level, EventId eventId, object state, Exception error)
        {
            Assert.Throws<ArgumentNullException>(() => sut.Log(level, eventId, state, error, null));
        }

        [Test, AutoMoqData]
        public void Logs_are_discarded_if_formatter_returns_null([Frozen] ILogglyProcessor processor, LogglyLogger sut, LogLevel level, EventId eventId, object state, Exception error)
        {
            Func<object, Exception, string> formatter = (s, ex) => null;

            sut.Log(level, eventId, state, error, formatter);

            Mock.Get(processor).Verify(p => p.EnqueueMessage(It.IsAny<LogglyMessage>()), Times.Never);
        }

        [Test, AutoMoqData]
        public void Logs_are_packed_into_LogglyMessage([Frozen] ILogglyProcessor processor, LogglyLogger sut, LogLevel level, EventId eventId, object state, Exception error, Func<object, Exception, string> formatter, string message)
        {
            Mock.Get(formatter).Setup(p => p(It.IsAny<object>(), It.IsAny<Exception>())).Returns(message);

            sut.Log(level, eventId, state, error, formatter);

            Mock.Get(processor).Verify(p => p.EnqueueMessage(It.Is<LogglyMessage>(m => m.Level == level && m.Category == sut.Name && m.EventId == eventId && m.Data == state && m.Message == message)));
        }

        [Test, AutoMoqData]
        public void No_error_is_added_to_message_when_exception_is_null([Frozen] ILogglyProcessor processor, LogglyLogger sut, LogLevel level, EventId eventId, object state, Func<object, Exception, string> formatter)
        {
            sut.Log(level, eventId, state, null, formatter);

            Mock.Get(processor).Verify(p => p.EnqueueMessage(It.Is<LogglyMessage>(o => o.Error == null)));
        }

        [Test, AutoMoqData]
        public void Messages_can_be_pre_processed([Frozen] ILogglyProcessor processor, [Frozen] LogglyOptions options, LogglyLogger sut, LogLevel level, EventId eventId, object state, Exception error, Func<object, Exception, string> formatter)
        {
            sut.Log(level, eventId, state, error, formatter);

            Mock.Get(options.PreProcessMessage).Verify(p => p(It.IsAny<LogglyMessage>()), Times.Once);
        }

        [Test, AutoMoqData]
        public void IsEnabled_uses_option_filter([Frozen] LogglyOptions options, LogglyLogger sut, LogLevel level)
        {
            sut.IsEnabled(level);

            Mock.Get(options.Filter).Verify(p => p(sut.Name, 0, level));
        }

        [Test, AutoMoqData]
        public void BeginScope_return_valid_disposable(LogglyLogger sut, object state)
        {
            var scope = sut.BeginScope(state);

            Assert.That(scope, Is.InstanceOf<IDisposable>());
        }

        [Test, AutoMoqData]
        public void BeginScope_return_valid_disposable_that_can_be_disposed(LogglyLogger sut, object state)
        {
            var scope = sut.BeginScope(state);
            scope.Dispose();

            Assert.That(scope, Is.InstanceOf<IDisposable>());
        }
    }
}