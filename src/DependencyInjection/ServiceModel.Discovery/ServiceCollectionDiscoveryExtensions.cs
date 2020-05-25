// ReSharper disable CheckNamespace

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using EMG.Extensions.DependencyInjection.Discovery;
using EMG.Extensions.DependencyInjection.Discovery.BindingCustomizations;
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

            services.Configure<ServiceModelDiscoveryOptions>(configurationSection);

            return services;
        }

        public static IServiceCollection ConfigureServiceDiscovery(this IServiceCollection services, Action<ServiceModelDiscoveryOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.Configure<ServiceModelDiscoveryOptions>(configureOptions);

            return services;
        }

        public static IServiceCollection AddServiceDiscovery(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();

            services.TryAddSingleton<IChannelFactoryWrapper, ChannelFactoryWrapper>();
            
            services.TryAddSingleton<IServiceModelDiscoveryClientWrapper, ServiceModelDiscoveryClientWrapper>();

            services.TryAddSingleton<IBindingFactory, CustomizableBindingFactory>();

            services.TryAddSingleton<IBindingFactoryCustomization>(new BindingFactoryCustomization(Uri.UriSchemeNetTcp, () => new NetTcpBinding()));

            services.TryAddSingleton<IBindingFactoryCustomization>(new BindingFactoryCustomization(Uri.UriSchemeHttp, () => new WSHttpBinding()));
            
            services.TryAddSingleton<IBindingFactoryCustomization>(new BindingFactoryCustomization(Uri.UriSchemeHttps, () => new WSHttpBinding()));

            services.TryAddSingleton<IDiscoveryService, ServiceModelDiscoveryService>();

            return services;
        }

        public static IServiceCollection AddServiceBindingCustomization<TService>(this IServiceCollection services, string uriScheme, Func<Binding> bindingFactory)
            where TService : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (uriScheme == null)
            {
                throw new ArgumentNullException(nameof(uriScheme));
            }

            if (bindingFactory == null)
            {
                throw new ArgumentNullException(nameof(bindingFactory));
            }

            services.Insert(0, ServiceDescriptor.Singleton<IBindingFactoryCustomization>(sp => new ServiceBindingFactoryCustomization<TService>(uriScheme, bindingFactory)));

            return services;
        }

        public static IServiceCollection AddBindingCustomization<TCustomization>(this IServiceCollection services)
            where TCustomization : class, IBindingFactoryCustomization
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Insert(0, ServiceDescriptor.Singleton<IBindingFactoryCustomization, TCustomization>());

            return services;
        }

        public static IServiceCollection DiscoverService<TService>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Transient) where TService : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Add(ServiceDescriptor.Describe(typeof(TService), ResolveService, serviceLifetime));

            return services;

            object ResolveService(IServiceProvider serviceProvider)
            {
                var discoveryService = serviceProvider.GetRequiredService<IDiscoveryService>();

                var service = discoveryService.Discover<TService>();

                return service;
            }
        }
    }
}
