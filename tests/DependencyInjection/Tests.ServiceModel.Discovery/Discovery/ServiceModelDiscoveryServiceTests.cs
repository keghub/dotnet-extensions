using System;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Extensions.DependencyInjection.Discovery;
using Moq;
using NUnit.Framework;

namespace Tests.Discovery
{
    [TestFixture]
    public class ServiceModelDiscoveryServiceTests
    {
        [Test]
        [CustomAutoData]
        public void Constructor_does_not_accept_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ServiceModelDiscoveryService).GetConstructors());
        }

        [Test]
        [CustomAutoData]
        public void Discover_uses_DiscoveryClientWrapper_to_find_valid_endpoints([Frozen] IServiceModelDiscoveryClientWrapper discoveryClient, ServiceModelDiscoveryService sut)
        {
            sut.Discover<ITestService>();

            Mock.Get(discoveryClient).Verify(p => p.FindEndpoints(It.IsAny<DiscoveryEndpoint>(), It.IsAny<FindCriteria>()));
        }

        [Test]
        [CustomAutoData]
        public void Discover_uses_Options_Binding_factory([Frozen] ServiceModelDiscoveryOptions options, ServiceModelDiscoveryService sut)
        {
            sut.Discover<ITestService>();

            Mock.Get(options.DiscoveryBindingFactory as Func<BasicHttpBinding>).Verify(p => p(), Times.Once());
        }
    }
}