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

            services.TryAddSingleton<IBindingFactory, CustomizableBindingFactory>();

            services.TryAddSingleton<IDiscoveryService, NetTcpDiscoveryAdapterService>();

            services.TryAddSingleton<IChannelFactoryWrapper, ChannelFactoryWrapper>();

            services.TryAddSingleton<IBindingFactoryCustomization>(new BindingFactoryCustomization(ServiceTypeSpecifications.AllServices, () => new NetTcpBinding()));

            return services;
        }

        public static IServiceCollection AddServiceBindingCustomization<TService>(this IServiceCollection services, Action<NetTcpBinding> bindingCustomization)
            where TService : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (bindingCustomization == null)
            {
                throw new ArgumentNullException(nameof(bindingCustomization));
            }

            services.Insert(0, ServiceDescriptor.Singleton<IBindingFactoryCustomization>(sp => new BindingFactoryCustomization(ServiceTypeSpecifications.ForService<TService>(), () =>
            {
                var binding = new NetTcpBinding();
                bindingCustomization?.Invoke(binding);
                return binding;
            })));

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

        public static IServiceCollection AddBindingCustomization(this IServiceCollection services, Action<NetTcpBinding> bindingCustomization)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (bindingCustomization == null)
            {
                throw new ArgumentNullException(nameof(bindingCustomization));
            }

            services.Insert(0, ServiceDescriptor.Singleton<IBindingFactoryCustomization>(sp => new BindingFactoryCustomization(ServiceTypeSpecifications.AllServices, () =>
            {
                var binding = new NetTcpBinding();
                bindingCustomization?.Invoke(binding);
                return binding;
            })));

            return services;
        }

        public static IServiceCollection DiscoverServiceUsingAdapter<TService>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
            where TService : class
        {
            services.Add(ServiceDescriptor.Describe(typeof(TService), ResolveService, serviceLifetime));

            return services;

            object ResolveService(IServiceProvider serviceProvider)
            {
                var discoveryService = serviceProvider.GetRequiredService<IDiscoveryService>();

                var bindingFactory = serviceProvider.GetRequiredService<IBindingFactory>();

                var binding = bindingFactory.Create(typeof(TService));

                var service = discoveryService.Discover<TService>(binding);

                return service;
            }
        }
    }
}
