using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Extensions.Logging.Loggly;
using Moq;
using NUnit.Framework;

namespace Tests.Loggly
{
    [TestFixture]
    public class LogglyLoggerProviderTests
    {
        [Test, AutoMoqData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(LogglyLoggerProvider).GetConstructors());
        }

        [Test, AutoMoqData]
        public void CreateLogger_returns_a_logger_with_given_category(LogglyLoggerProvider sut, string categoryName)
        {
            var logger = sut.CreateLogger(categoryName) as LogglyLogger;

            Assert.That(logger, Is.Not.Null);
            Assert.That(logger.Name, Is.EqualTo(categoryName));
        }

        [Test, AutoMoqData]
        public void Dispose_disposes_the_processor([Frozen] ILogglyProcessor processor, LogglyLoggerProvider sut)
        {
            sut.Dispose();

            Mock.Get(processor).Verify(p => p.Dispose());
        }
    }
}