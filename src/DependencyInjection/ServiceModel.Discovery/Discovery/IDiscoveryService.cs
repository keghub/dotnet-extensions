using System.ServiceModel.Channels;

namespace EMG.Extensions.DependencyInjection.Discovery
{
    public interface IDiscoveryService
    {
        TService Discover<TService>() where TService : class;
    }
}