using System;
using System.ServiceModel.Channels;

namespace EMG.Extensions.DependencyInjection.Discovery.BindingCustomizations
{
    public interface IBindingFactoryCustomization
    {
        Type ServiceType { get; }

        string UriScheme { get; }

        Binding Create();
    }
}