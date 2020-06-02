using System;
using System.ServiceModel.Channels;

namespace EMG.Extensions.DependencyInjection.Discovery.BindingCustomizations
{
    public class BindingFactoryCustomization : IBindingFactoryCustomization
    {
        private readonly Func<Binding> _bindingFactory;

        public BindingFactoryCustomization(string uriScheme, Func<Binding> bindingFactory)
        {
            UriScheme = uriScheme ?? throw new ArgumentNullException(nameof(uriScheme));
            _bindingFactory = bindingFactory ?? throw new ArgumentNullException(nameof(bindingFactory));
        }

        public Type ServiceType { get; } = null;

        public string UriScheme { get; }

        public Binding Create()
        {
            return _bindingFactory();
        }
    }
}