using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Discovery;
using System.Xml;
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
        private readonly ServiceModelDiscoveryOptions _options;

        public ServiceModelDiscoveryService(IOptions<ServiceModelDiscoveryOptions> options)
        {
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

            var criteria = new FindCriteria(typeof(TService));

            var response = discoveryClient.Find(criteria);

            var preferredEndpoint = response.Endpoints.FirstOrDefault(e => e.Address.Uri.Scheme == binding.Scheme);

            if (preferredEndpoint != null)
            {
                var channel = ChannelFactory<TService>.CreateChannel(binding, preferredEndpoint.Address);

                return channel;
            }

            return null;
        }
    }

}