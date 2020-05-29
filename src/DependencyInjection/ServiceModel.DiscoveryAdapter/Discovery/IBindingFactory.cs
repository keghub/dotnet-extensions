using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace EMG.Extensions.DependencyInjection.Discovery
{
    public interface IBindingFactory
    {
        NetTcpBinding Create(Type serviceType);
    }

    public class CustomizableBindingFactory : IBindingFactory
    {
        private readonly IReadOnlyList<IBindingFactoryCustomization> _customizations;

        public CustomizableBindingFactory(IEnumerable<IBindingFactoryCustomization> customizations)
        {
            _customizations = customizations?.ToArray() ?? throw new ArgumentNullException(nameof(customizations));
        }

        public NetTcpBinding Create(Type serviceType)
        {
            var matchingCustomizations = GetMatchingCustomizations(serviceType);

            var customization = matchingCustomizations.First();

            return customization.Create();
        }

        private IEnumerable<IBindingFactoryCustomization> GetMatchingCustomizations(Type serviceType)
        {
            foreach (var customization in _customizations.Where(c => c.ServiceSpecification.IsSatisfiedBy(serviceType)))
            {
                yield return customization;
            }

            foreach (var customization in _customizations.Where(c => c.ServiceSpecification.IsSatisfiedBy(null)))
            {
                yield return customization;
            }

            yield return BindingFactoryCustomization.Empty;
        }
    }
}