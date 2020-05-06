using System;
using System.Collections.Generic;
using AutoFixture.NUnit3;
using EMG.Extensions.Logging.Loggly;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
// ReSharper disable InvokeAsExtensionMethod

namespace Tests
{
    [TestFixture]
    public class LogglyLoggingBuilderExtensionsTests
    {
        [Test, AutoMoqData]
        public void AddLoggly_requires_ILoggingBuilder(Func<LogglyOptions> factoryDelegate)
        {
            Assert.Throws<ArgumentNullException>(() => LogglyLoggingBuilderExtensions.AddLoggly(null, factoryDelegate));
        }

        [Test, AutoMoqData]
        public void AddLoggly_requires_ILoggingBuilder(IConfigurationSection configuration, Action<LogglyOptions> configureOptions)
        {
            Assert.Throws<ArgumentNullException>(() => LogglyLoggingBuilderExtensions.AddLoggly(null, configuration, configureOptions));
        }

        [Test, AutoMoqData]
        public void AddLoggly_requires_FactoryDelegate(ILoggingBuilder builder)
        {
            Assert.Throws<ArgumentNullException>(() => LogglyLoggingBuilderExtensions.AddLoggly(builder, null));
        }

        [Test, AutoMoqData]
        public void AddLoggly_uses_factory_delegate(ILoggingBuilder builder, Func<LogglyOptions> factoryDelegate)
        {
            LogglyLoggingBuilderExtensions.AddLoggly(builder, factoryDelegate);

            Mock.Get(factoryDelegate).Verify(p => p());
        }

        [Test, AutoMoqData]
        public void AddLoggly_registers_options(ILoggingBuilder builder, [Frozen] LogglyOptions options, Func<LogglyOptions> factoryDelegate)
        {
            LogglyLoggingBuilderExtensions.AddLoggly(builder, factoryDelegate);

            Mock.Get(builder.Services).Verify(p => p.Add(It.Is<ServiceDescriptor>(sd => ReferenceEquals(sd.ImplementationInstance, options))));
        }

        [Test, AutoMoqData]
        public void AddLoggly_registers_an_ILogglyProcessor(ILoggingBuilder builder, Func<LogglyOptions> factoryDelegate)
        {
            LogglyLoggingBuilderExtensions.AddLoggly(builder, factoryDelegate);

            Mock.Get(builder.Services).Verify(p => p.Add(It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(ILogglyProcessor))));
        }

        [Test, AutoMoqData]
        public void AddLoggly_registers_an_ILogglyClient(ILoggingBuilder builder, Func<LogglyOptions> factoryDelegate)
        {
            LogglyLoggingBuilderExtensions.AddLoggly(builder, factoryDelegate);

            Mock.Get(builder.Services).Verify(p => p.Add(It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(ILogglyClient))));
        }

        [Test, AutoMoqData]
        public void AddLoggly_registers_an_ILoggerProvider(ILoggingBuilder builder, Func<LogglyOptions> factoryDelegate)
        {
            LogglyLoggingBuilderExtensions.AddLoggly(builder, factoryDelegate);

            Mock.Get(builder.Services).Verify(p => p.Add(It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(ILoggerProvider))));
        }

        [Test, AutoMoqData]
        public void AddLoggly_registers_options_with_ApiKey(ILoggingBuilder builder, [Frozen] LogglyOptions options, Func<LogglyOptions> factoryDelegate)
        {
            options.ApiKey = null;

            Assert.Throws<ArgumentNullException>(() => LogglyLoggingBuilderExtensions.AddLoggly(builder, factoryDelegate));
        }

        [Test, AutoMoqData]
        public void AddLoggly_uses_configureOptions_delegate(ILoggingBuilder builder, Action<LogglyOptions> configureOptions, string apiKey)
        {
            Mock.Get(configureOptions).Setup(p => p(It.IsAny<LogglyOptions>())).Callback<LogglyOptions>(o => o.ApiKey = apiKey);

            LogglyLoggingBuilderExtensions.AddLoggly(builder, configureOptions: configureOptions);
        }

        [Test, AutoMoqData]
        public void AddLoggly_uses_configuration(ILoggingBuilder builder, string apiKey)
        {
            var settings = new Dictionary<string, string>
            {
                ["ApiKey"] = apiKey
            };
            var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(settings);
            var configuration = configurationBuilder.Build();

            LogglyLoggingBuilderExtensions.AddLoggly(builder, configuration);

            Mock.Get(builder.Services).Verify(p => p.Add(It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(LogglyOptions) && (sd.ImplementationInstance as LogglyOptions).ApiKey == apiKey)));
        }

        [Test, AutoMoqData]
        public void AddLoggly_forwards_configuration_values_to_configureOptions_delegate(ILoggingBuilder builder, Action<LogglyOptions> configureOptions, string apiKey)
        {
            var settings = new Dictionary<string, string>
            {
                ["ApiKey"] = apiKey
            };
            var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(settings);
            var configuration = configurationBuilder.Build();

            LogglyLoggingBuilderExtensions.AddLoggly(builder, configuration, configureOptions);

            Mock.Get(configureOptions).Verify(p => p(It.Is<LogglyOptions>(lo => lo.ApiKey == apiKey)));
        }
    }
}