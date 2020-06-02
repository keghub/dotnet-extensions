using System;

namespace EMG.Extensions.DependencyInjection.Discovery
{
    public interface IServiceTypeSpecification
    {
        bool IsSatisfiedBy(Type serviceType);
    }

    public static class ServiceTypeSpecifications
    {
        public static IServiceTypeSpecification AllServices => new NoServiceTypeSpecification();

        public static IServiceTypeSpecification ForService<TService>() where TService : class => ForService(typeof(TService));

        public static IServiceTypeSpecification ForService(Type serviceType) => new ServiceTypeSpecification(serviceType);
    }

    public class ServiceTypeSpecification : IServiceTypeSpecification
    {
        private readonly Type _serviceType;

        public ServiceTypeSpecification(Type serviceType)
        {
            _serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        }

        public bool IsSatisfiedBy(Type serviceType)
        {
            return serviceType == _serviceType;
        }
    }

    public class NoServiceTypeSpecification : IServiceTypeSpecification
    {
        public bool IsSatisfiedBy(Type serviceType) => serviceType == null;
    }
}