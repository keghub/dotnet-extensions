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
        public Binding DiscoveryBinding { get; set; }

        public Uri ProbeEndpoint { get; set; }

        public bool IsValid() => string.Equals(DiscoveryBinding.Scheme, ProbeEndpoint.Scheme);
    }

    public class ServiceModelDiscoveryService : IDiscoveryService
    {
        private readonly ILogger<ServiceModelDiscoveryService> _logger;
        private readonly ServiceModelDiscoveryOptions _options;

        public ServiceModelDiscoveryService(IOptions<ServiceModelDiscoveryOptions> options, ILogger<ServiceModelDiscoveryService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

            if (!_options.IsValid())
            {
                throw new ArgumentException("ProbeEndpoint is not valid for the given DiscoveryBinding", nameof(ServiceModelDiscoveryOptions.ProbeEndpoint));
            }
        }

        public TService Discover<TService>(Binding binding) where TService : class
        {
            var probeEndpointAddress = new EndpointAddress(_options.ProbeEndpoint);
            var discoveryEndpoint = new DiscoveryEndpoint(_options.DiscoveryBinding, probeEndpointAddress);
            
            var discoveryClient = new DiscoveryClient(discoveryEndpoint);

            try
            {
                var criteria = new FindCriteria(typeof(TService));

                var response = discoveryClient.Find(criteria);

                discoveryClient.Close();

                var preferredEndpoint = response.Endpoints.FirstOrDefault(e => e.Address.Uri.Scheme == binding.Scheme);

                if (preferredEndpoint != null)
                {
                    var channel = ChannelFactory<TService>.CreateChannel(binding, preferredEndpoint.Address);

                    return channel;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while resolving the service {typeof(TService).Name}");
                discoveryClient.InnerChannel.Abort();
            }

            return null;
        }
    }

}