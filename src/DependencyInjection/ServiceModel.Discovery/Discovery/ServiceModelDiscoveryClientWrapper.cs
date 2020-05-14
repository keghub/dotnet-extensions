using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel.Discovery;

namespace EMG.Extensions.DependencyInjection.Discovery
{
    public interface IServiceModelDiscoveryClientWrapper
    {
        IReadOnlyList<EndpointDiscoveryMetadata> FindEndpoints(DiscoveryEndpoint endpoint, FindCriteria criteria);
    }

    public class ServiceModelDiscoveryClientWrapper : IServiceModelDiscoveryClientWrapper
    {
        public IReadOnlyList<EndpointDiscoveryMetadata> FindEndpoints(DiscoveryEndpoint endpoint, FindCriteria criteria)
        {
            var discoveryClient = new DiscoveryClient(endpoint);

            try
            {
                discoveryClient.Open();

                var response = discoveryClient.Find(criteria);

                discoveryClient.Close();

                return response.Endpoints;
            }
            catch (Exception)
            {
                discoveryClient.InnerChannel.Abort();
                throw;
            }
        }
    }
}