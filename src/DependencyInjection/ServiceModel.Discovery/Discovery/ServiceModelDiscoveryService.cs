using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Discovery;
using System.Xml;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EMG.Extensions.DependencyInjection.Discovery
{
    public class ServiceModelDiscoveryOptions
    {
        public Func<Binding> DiscoveryBindingFactory { get; set; }

        public Uri ProbeEndpoint { get; set; }
    }

    public class ServiceModelDiscoveryService : IDiscoveryService
    {
        private readonly IServiceModelDiscoveryClientWrapper _discoveryClient;
        private readonly IChannelFactoryWrapper _channelFactory;
        private readonly ILogger<ServiceModelDiscoveryService> _logger;
        private readonly ServiceModelDiscoveryOptions _options;

        public ServiceModelDiscoveryService(IServiceModelDiscoveryClientWrapper discoveryClient, IChannelFactoryWrapper channelFactory, IOptions<ServiceModelDiscoveryOptions> options, ILogger<ServiceModelDiscoveryService> logger)
        {
            _discoveryClient = discoveryClient ?? throw new ArgumentNullException(nameof(discoveryClient));
            _channelFactory = channelFactory ?? throw new ArgumentNullException(nameof(channelFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public TService Discover<TService>(Binding binding) where TService : class
        {
            var discoveryBinding = _options.DiscoveryBindingFactory.Invoke();

            if (!string.Equals(discoveryBinding.Scheme, _options.ProbeEndpoint.Scheme))
            {
                throw new ArgumentException("ProbeEndpoint is not valid for the given DiscoveryBinding", nameof(ServiceModelDiscoveryOptions.ProbeEndpoint));
            }

            var probeEndpointAddress = new EndpointAddress(_options.ProbeEndpoint);
            var discoveryEndpoint = new DiscoveryEndpoint(discoveryBinding, probeEndpointAddress);

            try
            {
                var criteria = new FindCriteria(typeof(TService));

                var endpoints = _discoveryClient.FindEndpoints(discoveryEndpoint, criteria);

                var preferredEndpoint = endpoints?.FirstOrDefault(e => e.Address.Uri.Scheme == binding.Scheme);

                if (preferredEndpoint != null)
                {
                    return _channelFactory.CreateChannel<TService>(binding, preferredEndpoint.Address);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while resolving the service {typeof(TService).Name}");
            }

            return null;
        }
    }
}