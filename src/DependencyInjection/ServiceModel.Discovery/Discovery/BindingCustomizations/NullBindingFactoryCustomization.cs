using System;
using System.ServiceModel.Channels;

namespace EMG.Extensions.DependencyInjection.Discovery.BindingCustomizations
{
    public class NullBindingFactoryCustomization : IBindingFactoryCustomization
    {
        public static readonly IBindingFactoryCustomization Default = new NullBindingFactoryCustomization();
        private NullBindingFactoryCustomization() { }

        public Type ServiceType { get; } = null;

        public string UriScheme { get; } = null;

        public Binding Create()
        {
            return null;
        }
    }
}