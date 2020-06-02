using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using AutoFixture.Idioms;
using EMG.Extensions.DependencyInjection.Discovery;
using EMG.Extensions.DependencyInjection.Discovery.BindingCustomizations;
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
            services.AddServiceDiscovery();

            var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetRequiredService<IDiscoveryService>();
        }

        [Test, CustomAutoData]
        public void AddServiceDiscovery_does_not_accept_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ServiceCollectionDiscoveryExtensions).GetMethod(nameof(ServiceCollectionDiscoveryExtensions.AddServiceDiscovery)));
        }

        [Test, CustomAutoData]
        public void ConfigureServiceDiscovery_configures_options_with_delegate(ServiceCollection services, Action<ServiceModelDiscoveryOptions> configureOptions)
        {
            services.ConfigureServiceDiscovery(configureOptions);

            var serviceProvider = services.BuildServiceProvider();

            var options = serviceProvider.GetRequiredService<IOptions<ServiceModelDiscoveryOptions>>();

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

            var options = serviceProvider.GetRequiredService<IOptions<ServiceModelDiscoveryOptions>>();

            Assert.That(options.Value.ProbeEndpoint, Is.EqualTo(probeEndpoint));
        }

        [Test, CustomAutoData]
        public void ConfigureServiceDiscovery_does_not_accept_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ServiceCollectionDiscoveryExtensions).GetRuntimeMethods().Where(m => m.Name == nameof(ServiceCollectionDiscoveryExtensions.ConfigureServiceDiscovery)));
        }

        [Test, CustomAutoData]
        public void AddServiceBindingCustomization_does_not_accept_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ServiceCollectionDiscoveryExtensions).GetRuntimeMethods().Where(m => m.Name == nameof(ServiceCollectionDiscoveryExtensions.AddServiceBindingCustomization)));
        }

        [Test, CustomAutoData]
        public void DiscoverService_does_not_accept_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ServiceCollectionDiscoveryExtensions).GetRuntimeMethods().Where(m => m.Name == nameof(ServiceCollectionDiscoveryExtensions.DiscoverService)));
        }

        [Test, CustomAutoData]
        public void DiscoverService_registers_service_using_discovery(ServiceCollection services, ServiceLifetime lifetime, ITestService testService, IDiscoveryService discoveryService)
        {
            services.DiscoverService<ITestService>(lifetime);

            services.AddSingleton<IDiscoveryService>(discoveryService);

            Mock.Get(discoveryService).Setup(p => p.Discover<ITestService>()).Returns(testService);

            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<ITestService>();

            Assert.That(service, Is.SameAs(testService));
        }

        [Test, CustomAutoData]
        public void AddServiceBindingCustomization_registers_a_customization_with_given_criteria(ServiceCollection services, string uriScheme, Func<Binding> bindingFactory)
        {
            services.AddServiceBindingCustomization<ITestService>(uriScheme, bindingFactory);

            var serviceProvider = services.BuildServiceProvider();

            var customization = serviceProvider.GetRequiredService<IBindingFactoryCustomization>();

            Assert.That(customization.ServiceType, Is.EqualTo(typeof(ITestService)));

            Assert.That(customization.UriScheme, Is.EqualTo(uriScheme));
        }

        [Test, CustomAutoData]
        public void AddServiceBindingCustomization_registers_a_customization_using_given_factory_method(ServiceCollection services, string uriScheme, Func<Binding> bindingFactory)
        {
            services.AddServiceBindingCustomization<ITestService>(uriScheme, bindingFactory);

            var serviceProvider = services.BuildServiceProvider();

            var customization = serviceProvider.GetRequiredService<IBindingFactoryCustomization>();

            _ = customization.Create();

            Mock.Get(bindingFactory as Func<BasicHttpBinding>).Verify(p => p(), Times.Once);
        }

        [Test, CustomAutoData]
        public void AddBindingCustomization_does_not_accept_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ServiceCollectionDiscoveryExtensions).GetRuntimeMethods().Where(m => m.Name == nameof(ServiceCollectionDiscoveryExtensions.AddBindingCustomization)));
        }

        [Test, CustomAutoData]
        public void AddBindingCustomization_registers_a_customization_by_its_type(ServiceCollection services)
        {
            services.AddBindingCustomization<TestCustomization>();

            var serviceProvider = services.BuildServiceProvider();

            var customization = serviceProvider.GetRequiredService<IBindingFactoryCustomization>();

            Assert.That(customization, Is.InstanceOf<TestCustomization>());
        }
    }
}