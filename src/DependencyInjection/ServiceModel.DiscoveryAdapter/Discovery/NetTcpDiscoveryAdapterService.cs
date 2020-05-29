using System;
using System.Reflection;
using System.ServiceModel;
using System.Xml;
using EMG.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EMG.Extensions.DependencyInjection.Discovery
{
    public class NetTcpDiscoveryOptions
    {
        public Uri ProbeEndpoint { get; set; }

        public Action<NetTcpBinding> ConfigureDiscoveryAdapterBinding { get; set; } = delegate { };
    }

    public class NetTcpDiscoveryAdapterService : IDiscoveryService
    {
        private readonly IChannelFactoryWrapper _channelFactory;
        private readonly ILogger<NetTcpDiscoveryAdapterService> _logger;
        private readonly NetTcpDiscoveryOptions _options;

        public NetTcpDiscoveryAdapterService(IChannelFactoryWrapper channelFactory, IOptions<NetTcpDiscoveryOptions> options, ILogger<NetTcpDiscoveryAdapterService> logger)
        {
            _channelFactory = channelFactory ?? throw new ArgumentNullException(nameof(channelFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public TService Discover<TService>(NetTcpBinding binding) where TService : class
        {
            if (!TryGetTargetEndpointAddress(typeof(TService), out var endpointAddress))
            {
                _logger.LogError($"Impossible to resolve service: {typeof(TService).Name}");
                return null;
            }

            return _channelFactory.CreateChannel<TService>(binding, endpointAddress);
        }

        private bool TryGetTargetEndpointAddress(Type serviceType, out EndpointAddress endpointAddress)
        {
            endpointAddress = null;

            var channel = GetDiscoveryServiceAdapter();

            try
            {
                var serviceQualifiedName = GetQualifiedName(serviceType);

                var endpoint = channel.Discover(serviceQualifiedName);

                (channel as ICommunicationObject).Close();

                if (string.Equals(endpoint?.Scheme, Uri.UriSchemeNetTcp, StringComparison.OrdinalIgnoreCase))
                {
                    endpointAddress = new EndpointAddress(endpoint);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while resolving the service {serviceType.Name}");
                (channel as ICommunicationObject).Abort();
                return false;
            }

            return false;
        }

        private IDiscoveryAdapter GetDiscoveryServiceAdapter()
        {
            if (_options.ProbeEndpoint == null)
            {
                throw new ArgumentNullException(nameof(_options.ProbeEndpoint), $"{nameof(NetTcpDiscoveryOptions.ProbeEndpoint)} cannot be null. Please configure {nameof(NetTcpDiscoveryOptions)}");
            }

            var binding = new NetTcpBinding();
            _options.ConfigureDiscoveryAdapterBinding?.Invoke(binding);

            var address = new EndpointAddress(_options.ProbeEndpoint);

            return _channelFactory.CreateChannel<IDiscoveryAdapter>(binding, address);
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