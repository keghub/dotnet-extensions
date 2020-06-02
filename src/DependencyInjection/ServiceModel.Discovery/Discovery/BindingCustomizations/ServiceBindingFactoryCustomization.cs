using System;
using System.ServiceModel.Channels;

namespace EMG.Extensions.DependencyInjection.Discovery.BindingCustomizations
{
    public class ServiceBindingFactoryCustomization<TService> : IBindingFactoryCustomization
        where TService : class
    {
        private readonly Func<Binding> _bindingFactory;

        public ServiceBindingFactoryCustomization(string uriScheme, Func<Binding> bindingFactory)
        {
            _bindingFactory = bindingFactory ?? throw new ArgumentNullException(nameof(bindingFactory));
            UriScheme = uriScheme ?? throw new ArgumentNullException(nameof(uriScheme));
        }

        public Type ServiceType { get; } = typeof(TService);

        public string UriScheme { get; }

        public Binding Create()
        {
            return _bindingFactory();
        }
    }
}