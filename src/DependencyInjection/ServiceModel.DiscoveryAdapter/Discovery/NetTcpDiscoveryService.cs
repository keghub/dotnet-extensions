using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using EMG.Utilities;
using Microsoft.Extensions.Options;

namespace EMG.Extensions.DependencyInjection.Discovery
{
    public class NetTcpDiscoveryOptions
    {
        public string ProbeEndpoint { get; set; }

        public Action<NetTcpBinding> ConfigureDiscoveryAdapterBinding { get; set; } = delegate { };
    }

    public class NetTcpDiscoveryAdapterService : IDiscoveryService
    {
        private readonly NetTcpDiscoveryOptions _options;

        public NetTcpDiscoveryAdapterService(IOptions<NetTcpDiscoveryOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public TService Discover<TService>(Binding binding) where TService : class
        {
            if (!TryGetTargetEndpointAddress(typeof(TService), out var endpointAddress))
            {
                return null;
            }

            var factory = new ChannelFactory<TService>(binding, endpointAddress);

            return factory.CreateChannel();
        }

        private bool TryGetTargetEndpointAddress(Type serviceType, out EndpointAddress endpointAddress)
        {
            endpointAddress = null;

            var channel = GetDiscoveryServiceAdapter();

            try
            {
                var serviceQualifiedName = GetQualifiedName(serviceType);

                var endpoint = channel.Discover(serviceQualifiedName);

                if (endpoint != null)
                {
                    endpointAddress = new EndpointAddress(endpoint);
                    return true;
                }
            }
            catch (CommunicationException ex)
            {
                throw new EndpointNotFoundException($"Could not retrieve endpoint for {serviceType.Name}.", ex);
            }
            catch
            {
                return false;
            }
            finally
            {
                (channel as ICommunicationObject).Close();
            }

            return false;
        }

        private IDiscoveryAdapter GetDiscoveryServiceAdapter()
        {
            var binding = new NetTcpBinding();
            _options.ConfigureDiscoveryAdapterBinding?.Invoke(binding);

            var address = new EndpointAddress(_options.ProbeEndpoint);

            var factory = new ChannelFactory<IDiscoveryAdapter>(binding, address);

            var channel = factory.CreateChannel();

            return channel;
        }

        private const string DefaultNamespace = "http://tempuri.org/";

        private XmlQualifiedName GetQualifiedName(Type serviceType)
        {
            var contractAttribute = serviceType.GetTypeInfo().GetCustomAttribute<ServiceContractAttribute>();

            var ns = contractAttribute?.Namespace ?? DefaultNamespace;
            var name = contractAttribute?.Name ?? serviceType.Name;

            return new XmlQualifiedName(name, ns);
        }
    }
}