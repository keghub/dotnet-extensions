using System;
using EMG.Extensions.Logging.Loggly;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class LogglyLoggingBuilderExtensionsIntegrationTests
    {
        [Test, AutoMoqData]
        public void AddLoggly_supports_registration(string categoryName, Func<LogglyOptions> optionsFactory)
        {
            var services = new ServiceCollection();

            services.AddLogging(builder => builder.AddLoggly(optionsFactory));

            var serviceProvider = services.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<ILoggerFactory>();

            var logger = factory.CreateLogger(categoryName);

            Assert.That(logger, Is.Not.Null);
        }

        [Test, AutoMoqData]
        public void AddLoggly_registers_LogglyLoggerProvider(Func<LogglyOptions> optionsFactory)
        {
            var services = new ServiceCollection();

            services.AddLogging(builder => builder.AddLoggly(optionsFactory));

            var serviceProvider = services.BuildServiceProvider();

            var provider = serviceProvider.GetRequiredService<ILoggerProvider>() as LogglyLoggerProvider;

            Assert.That(provider, Is.Not.Null);
        }
    }
}