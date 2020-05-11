// ReSharper disable CheckNamespace

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using EMG.Extensions.DependencyInjection.Discovery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionDiscoveryExtensions
    {
        public static IServiceCollection AddServiceDiscoveryAdapter(this IServiceCollection services, IConfigurationSection configurationSection, Action<NetTcpDiscoveryOptions> configureOptions = null)
        {
            services.TryAddSingleton<IDiscoveryService, NetTcpDiscoveryAdapterService>();

            services.Configure<NetTcpDiscoveryOptions>(configurationSection);

            if (configureOptions != null)
            {
                services.Configure<NetTcpDiscoveryOptions>(configureOptions);
            }

            return services;
        }

        public static IServiceCollection DiscoverServiceUsingAdapter<TService>(this IServiceCollection services, Action<NetTcpBinding> customizeBinding = null, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
            where TService : class
        {
            services.Add(ServiceDescriptor.Describe(typeof(TService), ResolveService, serviceLifetime));

            return services;

            object ResolveService(IServiceProvider serviceProvider)
            {
                var discoveryService = serviceProvider.GetRequiredService<IDiscoveryService>();

                var binding = new NetTcpBinding();

                customizeBinding?.Invoke(binding);

                var service = discoveryService.Discover<TService>(binding);

                return service;
            }
        }
    }
}
