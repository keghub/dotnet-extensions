using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Extensions.DependencyInjection.Discovery;
using EMG.Utilities;
using Moq;
using NUnit.Framework;

namespace Tests.Discovery
{
    [TestFixture]
    public class NetTcpDiscoveryAdapterServiceTests
    {
        [Test, CustomAutoData]
        public void Constructor_does_not_accept_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(NetTcpDiscoveryAdapterService).GetConstructors());
        }

        [Test, CustomAutoData]
        public void Discover_returns_a_service_proxy([Frozen] IChannelFactoryWrapper channelFactory, NetTcpDiscoveryAdapterService sut, NetTcpBinding serviceBinding, ITestService testService, IDiscoveryAdapter discoveryAdapter)
        {
            Mock.Get(channelFactory).Setup(p => p.CreateChannel<IDiscoveryAdapter>(It.IsAny<Binding>(), It.IsAny<EndpointAddress>())).Returns(discoveryAdapter);

            Mock.Get(channelFactory).Setup(p => p.CreateChannel<ITestService>(It.IsAny<NetTcpBinding>(), It.IsAny<EndpointAddress>())).Returns(testService);

            var service = sut.Discover<ITestService>(serviceBinding);

            Assert.That(service, Is.Not.Null);
        }

        [Test, CustomAutoData]
        public void Discover_uses_channel_factory_wrapper_to_access_adapter([Frozen] IChannelFactoryWrapper channelFactory, [Frozen] NetTcpDiscoveryOptions options, NetTcpDiscoveryAdapterService sut, NetTcpBinding serviceBinding, ITestService testService, IDiscoveryAdapter discoveryAdapter)
        {
            Mock.Get(channelFactory).Setup(p => p.CreateChannel<IDiscoveryAdapter>(It.IsAny<Binding>(), It.IsAny<EndpointAddress>())).Returns(discoveryAdapter);

            Mock.Get(channelFactory).Setup(p => p.CreateChannel<ITestService>(It.IsAny<NetTcpBinding>(), It.IsAny<EndpointAddress>())).Returns(testService);

            _ = sut.Discover<ITestService>(serviceBinding);

            Mock.Get(channelFactory).Verify(p => p.CreateChannel<IDiscoveryAdapter>(It.IsAny<NetTcpBinding>(), It.Is<EndpointAddress>(ea => ea.Uri == options.ProbeEndpoint)));
        }

        [Test, CustomAutoData]
        public void Discover_uses_channel_factory_wrapper_to_create_connection_to_service([Frozen] IChannelFactoryWrapper channelFactory, NetTcpDiscoveryAdapterService sut, NetTcpBinding serviceBinding, ITestService testService, IDiscoveryAdapter discoveryAdapter, Uri serviceAddress)
        {
            Mock.Get(channelFactory).Setup(p => p.CreateChannel<IDiscoveryAdapter>(It.IsAny<Binding>(), It.IsAny<EndpointAddress>())).Returns(discoveryAdapter);

            Mock.Get(channelFactory).Setup(p => p.CreateChannel<ITestService>(It.IsAny<NetTcpBinding>(), It.IsAny<EndpointAddress>())).Returns(testService);

            Mock.Get(discoveryAdapter).Setup(p => p.Discover(It.IsAny<XmlQualifiedName>())).Returns(serviceAddress);

            _ = sut.Discover<ITestService>(serviceBinding);

            Mock.Get(channelFactory).Setup(p => p.CreateChannel<ITestService>(serviceBinding, It.Is<EndpointAddress>(ea => ea.Uri == serviceAddress))).Returns(testService);
        }

        [Test, CustomAutoData]
        public void No_service_is_discovered_if_adapter_throws([Frozen] IChannelFactoryWrapper channelFactory, NetTcpDiscoveryAdapterService sut, NetTcpBinding serviceBinding, IDiscoveryAdapter discoveryAdapter, Exception error)
        {
            Mock.Get(channelFactory).Setup(p => p.CreateChannel<IDiscoveryAdapter>(It.IsAny<Binding>(), It.IsAny<EndpointAddress>())).Returns(discoveryAdapter);

            Mock.Get(discoveryAdapter).Setup(p => p.Discover(It.IsAny<XmlQualifiedName>())).Throws(error);

            var service = sut.Discover<ITestService>(serviceBinding);

            Assert.That(service, Is.Null);
        }

        [Test, CustomAutoData]
        public void No_service_is_discovered_if_adapter_returns_nothing([Frozen] IChannelFactoryWrapper channelFactory, NetTcpDiscoveryAdapterService sut, NetTcpBinding serviceBinding, IDiscoveryAdapter discoveryAdapter, ITestService testService)
        {
            Mock.Get(channelFactory).Setup(p => p.CreateChannel<IDiscoveryAdapter>(It.IsAny<Binding>(), It.IsAny<EndpointAddress>())).Returns(discoveryAdapter);

            Mock.Get(channelFactory).Setup(p => p.CreateChannel<ITestService>(It.IsAny<NetTcpBinding>(), It.IsAny<EndpointAddress>())).Returns(testService);

            Mock.Get(discoveryAdapter).Setup(p => p.Discover(It.IsAny<XmlQualifiedName>())).Returns(null as Uri);

            var service = sut.Discover<ITestService>(serviceBinding);

            Assert.That(service, Is.Null);
        }

        [Test, CustomAutoData]
        public void Probe_endpoint_cant_be_null([Frozen] IChannelFactoryWrapper channelFactory, [Frozen] NetTcpDiscoveryOptions options, NetTcpDiscoveryAdapterService sut, NetTcpBinding serviceBinding, ITestService testService, IDiscoveryAdapter discoveryAdapter)
        {
            Mock.Get(channelFactory).Setup(p => p.CreateChannel<IDiscoveryAdapter>(It.IsAny<Binding>(), It.IsAny<EndpointAddress>())).Returns(discoveryAdapter);

            Mock.Get(channelFactory).Setup(p => p.CreateChannel<ITestService>(It.IsAny<NetTcpBinding>(), It.IsAny<EndpointAddress>())).Returns(testService);

            options.ProbeEndpoint = null;

            Assert.Throws<ArgumentNullException>(() => sut.Discover<ITestService>(serviceBinding));
        }
    }
}
