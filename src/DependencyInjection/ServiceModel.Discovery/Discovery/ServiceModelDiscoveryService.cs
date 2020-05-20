using System;
using System.Collections.Generic;
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
        private readonly IBindingFactory _bindingFactory;
        private readonly ILogger<ServiceModelDiscoveryService> _logger;
        private readonly ServiceModelDiscoveryOptions _options;

        public ServiceModelDiscoveryService(IServiceModelDiscoveryClientWrapper discoveryClient, IChannelFactoryWrapper channelFactory, IBindingFactory bindingFactory, IOptions<ServiceModelDiscoveryOptions> options, ILogger<ServiceModelDiscoveryService> logger)
        {
            _discoveryClient = discoveryClient ?? throw new ArgumentNullException(nameof(discoveryClient));
            _channelFactory = channelFactory ?? throw new ArgumentNullException(nameof(channelFactory));
            _bindingFactory = bindingFactory ?? throw new ArgumentNullException(nameof(bindingFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public TService Discover<TService>() where TService : class
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

                var items = from endpoint in endpoints
                            let binding = _bindingFactory.Create(typeof(TService), endpoint.Address.Uri.Scheme)
                            where binding != null
                            let channel = _channelFactory.CreateChannel<TService>(binding, endpoint.Address)
                            select channel;

                return items.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while resolving the service {typeof(TService).Name}");
            }

            return null;
        }
    }
}