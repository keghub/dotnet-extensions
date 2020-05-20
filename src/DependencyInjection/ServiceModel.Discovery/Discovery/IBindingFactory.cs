using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using EMG.Extensions.DependencyInjection.Discovery.BindingCustomizations;

namespace EMG.Extensions.DependencyInjection.Discovery 
{
    public interface IBindingFactory
    {
        Binding Create(Type serviceType, string scheme);
    }

    public class CustomizableBindingFactory : IBindingFactory
    {
        private readonly IReadOnlyList<IBindingFactoryCustomization> _bindingFactoryCustomizations;

        public CustomizableBindingFactory(IEnumerable<IBindingFactoryCustomization> bindingFactoryCustomizations)
        {
            _bindingFactoryCustomizations = bindingFactoryCustomizations?.ToArray() ?? throw new ArgumentNullException(nameof(bindingFactoryCustomizations));
        }

        public Binding Create(Type serviceType, string scheme)
        {
            var matchingCustomizations = GetMatchingCustomizations(serviceType, scheme);

            var customization = matchingCustomizations.First();

            return customization.Create();
        }

        private IEnumerable<IBindingFactoryCustomization> GetMatchingCustomizations(Type serviceType, string scheme)
        {
            foreach (var customization in _bindingFactoryCustomizations.Where(c => c.ServiceType == serviceType && string.Equals(c.UriScheme, scheme, StringComparison.OrdinalIgnoreCase)))
                yield return customization;

            foreach (var customization in _bindingFactoryCustomizations.Where(c => c.ServiceType == null && string.Equals(c.UriScheme, scheme, StringComparison.OrdinalIgnoreCase)))
                yield return customization;

            yield return NullBindingFactoryCustomization.Default;
        }
    }
}