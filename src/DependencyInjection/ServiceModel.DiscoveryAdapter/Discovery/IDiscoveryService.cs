using System.ServiceModel;

namespace EMG.Extensions.DependencyInjection.Discovery
{
    public interface IDiscoveryService
    {
        TService Discover<TService>(NetTcpBinding binding) where TService : class;
    }
}