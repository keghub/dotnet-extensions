// ReSharper disable CheckNamespace

using System;
using System.ServiceModel;
using EMG.Extensions.DependencyInjection.Discovery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionDiscoveryExtensions
    {
        public static IServiceCollection ConfigureServiceDiscovery(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configurationSection == null)
            {
                throw new ArgumentNullException(nameof(configurationSection));
            }

            services.Configure<NetTcpDiscoveryOptions>(configurationSection);

            return services;
        }

        public static IServiceCollection ConfigureServiceDiscovery(this IServiceCollection services, Action<NetTcpDiscoveryOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.Configure<NetTcpDiscoveryOptions>(configureOptions);

            return services;
        }

        public static IServiceCollection AddServiceDiscoveryAdapter(this IServiceCollection services)
        {
            services.AddOptions();

            services.TryAddSingleton<IDiscoveryService, NetTcpDiscoveryAdapterService>();

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
