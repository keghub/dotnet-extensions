using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using AutoFixture.Idioms;
using EMG.Extensions.DependencyInjection.Discovery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Tests
{
    public class ServiceCollectionDiscoveryExtensionsTests
    {
        [Test, CustomAutoData]
        public void AddServiceDiscovery_registers_IDiscoveryService(ServiceCollection services)
        {
            services.AddServiceDiscoveryAdapter();

            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<IDiscoveryService>();

            Assert.That(service, Is.Not.Null);
        }

        [Test, CustomAutoData]
        public void AddServiceDiscovery_registers_IBindingFactory(ServiceCollection services)
        {
            services.AddServiceDiscoveryAdapter();

            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<IBindingFactory>();

            Assert.That(service, Is.Not.Null);
        }

        [Test, CustomAutoData]
        public void AddServiceDiscovery_does_not_accept_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ServiceCollectionDiscoveryExtensions).GetMethod(nameof(ServiceCollectionDiscoveryExtensions.AddServiceDiscoveryAdapter)));
        }

        [Test, CustomAutoData]
        public void ConfigureServiceDiscovery_configures_options_with_delegate(ServiceCollection services, Action<NetTcpDiscoveryOptions> configureOptions)
        {
            services.ConfigureServiceDiscovery(configureOptions);

            var serviceProvider = services.BuildServiceProvider();

            var options = serviceProvider.GetRequiredService<IOptions<NetTcpDiscoveryOptions>>();

            Mock.Get(configureOptions).Verify(p => p(options.Value));
        }

        [Test, CustomAutoData]
        public void ConfigureServiceDiscovery_configures_options_with_configuration(ServiceCollection services, ConfigurationBuilder configurationBuilder, string sectionName, Uri probeEndpoint)
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                [$"{sectionName}:ProbeEndpoint"] = probeEndpoint.ToString()
            });

            var configuration = configurationBuilder.Build();

            services.ConfigureServiceDiscovery(configuration.GetSection(sectionName));

            var serviceProvider = services.BuildServiceProvider();

            var options = serviceProvider.GetRequiredService<IOptions<NetTcpDiscoveryOptions>>();

            Assert.That(options.Value.ProbeEndpoint, Is.EqualTo(probeEndpoint));
        }

        [Test, CustomAutoData]
        public void ConfigureServiceDiscovery_does_not_accept_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ServiceCollectionDiscoveryExtensions).GetRuntimeMethods().Where(m => m.Name == nameof(ServiceCollectionDiscoveryExtensions.ConfigureServiceDiscovery)));
        }

        [Test, CustomAutoData]
        public void DiscoverServiceUsingAdapter_registers_service_using_discovery(ServiceCollection services, ServiceLifetime lifetime, ITestService testService, IDiscoveryService discoveryService, IBindingFactory bindingFactory)
        {
            services.DiscoverServiceUsingAdapter<ITestService>(lifetime);

            services.AddSingleton<IBindingFactory>(bindingFactory);

            services.AddSingleton<IDiscoveryService>(discoveryService);

            Mock.Get(discoveryService).Setup(p => p.Discover<ITestService>(It.IsAny<NetTcpBinding>())).Returns(testService);

            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<ITestService>();

            Assert.That(service, Is.SameAs(testService));
        }

        [Test, CustomAutoData]
        public void AddServiceBindingCustomization_does_not_accept_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ServiceCollectionDiscoveryExtensions).GetRuntimeMethods().Where(m => m.Name == nameof(ServiceCollectionDiscoveryExtensions.AddServiceBindingCustomization)));
        }

        [Test, CustomAutoData]
        public void AddServiceBindingCustomization_registers_binding_customization_for_service(ServiceCollection services, Action<NetTcpBinding> bindingCustomization)
        {
            services.AddServiceBindingCustomization<ITestService>(bindingCustomization);

            var serviceProvider = services.BuildServiceProvider();

            var customization = serviceProvider.GetRequiredService<IBindingFactoryCustomization>();

            _ = customization.Create();

            Mock.Get(bindingCustomization).Verify(p => p(It.IsAny<NetTcpBinding>()), Times.Once());
        }

        [Test, CustomAutoData]
        public void AddBindingCustomization_does_not_accept_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ServiceCollectionDiscoveryExtensions).GetRuntimeMethods().Where(m => m.Name == nameof(ServiceCollectionDiscoveryExtensions.AddBindingCustomization)));
        }

        [Test, CustomAutoData]
        public void AddBindingCustomization_registers_customization_by_type(ServiceCollection services)
        {
            services.AddBindingCustomization<TestCustomization>();

            var serviceProvider = services.BuildServiceProvider();

            var customization = serviceProvider.GetRequiredService<IBindingFactoryCustomization>();

            Assert.That(customization, Is.InstanceOf<TestCustomization>());
        }

        [Test, CustomAutoData]
        public void AddBindingCustomization_registers_binding_customization(ServiceCollection services, Action<NetTcpBinding> bindingCustomization)
        {
            services.AddBindingCustomization(bindingCustomization);

            var serviceProvider = services.BuildServiceProvider();

            var customization = serviceProvider.GetRequiredService<IBindingFactoryCustomization>();

            _ = customization.Create();

            Mock.Get(bindingCustomization).Verify(p => p(It.IsAny<NetTcpBinding>()), Times.Once());
        }
    }
}