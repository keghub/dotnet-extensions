using System;
using System.ServiceModel;

namespace EMG.Extensions.DependencyInjection.Discovery 
{
    public interface IBindingFactoryCustomization
    {
        IServiceTypeSpecification ServiceSpecification { get; }

        NetTcpBinding Create();
    }

    public class BindingFactoryCustomization : IBindingFactoryCustomization
    {
        public static readonly IBindingFactoryCustomization Empty = new BindingFactoryCustomization(ServiceTypeSpecifications.AllServices, () => null);

        private readonly Func<NetTcpBinding> _bindingFactory;

        public BindingFactoryCustomization(IServiceTypeSpecification specification, Func<NetTcpBinding> bindingFactory)
        {
            ServiceSpecification = specification ?? throw new ArgumentNullException(nameof(specification));
            _bindingFactory = bindingFactory ?? throw new ArgumentNullException(nameof(bindingFactory));
        }

        public IServiceTypeSpecification ServiceSpecification { get; }

        public NetTcpBinding Create()
        {
            return _bindingFactory();
        }
    }
}