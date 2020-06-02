using System;
using System.ServiceModel;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Extensions.DependencyInjection.Discovery;
using Moq;
using NUnit.Framework;

namespace Tests.Discovery 
{
    [TestFixture]
    public class CustomizableBindingFactoryTests
    {
        [SetUp]
        public void Initialize()
        {
            
        }

        [Test, CustomAutoData]
        public void Constructor_does_not_accept_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(CustomizableBindingFactory).GetConstructors());
        }

        

        [Test, CustomAutoData]
        public void Create_uses_service_specification_if_available(Func<NetTcpBinding> serviceBindingFactory, Func<NetTcpBinding> openBindingFactory)
        {
            var serviceMatchingCustomization = new BindingFactoryCustomization(ServiceTypeSpecifications.ForService(typeof(ITestService)), serviceBindingFactory);
            var openCustomization = new BindingFactoryCustomization(ServiceTypeSpecifications.AllServices, openBindingFactory);

            var sut = new CustomizableBindingFactory(new[] { serviceMatchingCustomization, openCustomization });

            _ = sut.Create(typeof(ITestService));

            Mock.Get(serviceBindingFactory).Verify(p => p(), Times.Once);
            Mock.Get(openBindingFactory).Verify(p => p(), Times.Never);
        }

        [Test, CustomAutoData]
        public void Create_uses_generic_specification_if_no_matching_service_spec_is_available(Func<NetTcpBinding> serviceBindingFactory, Func<NetTcpBinding> openBindingFactory, Type anotherServiceType)
        {
            var serviceMatchingCustomization = new BindingFactoryCustomization(ServiceTypeSpecifications.ForService(anotherServiceType), serviceBindingFactory);
            var openCustomization = new BindingFactoryCustomization(ServiceTypeSpecifications.AllServices, openBindingFactory);

            var sut = new CustomizableBindingFactory(new[] { serviceMatchingCustomization, openCustomization });

            _ = sut.Create(typeof(ITestService));

            Mock.Get(serviceBindingFactory).Verify(p => p(), Times.Never);
            Mock.Get(openBindingFactory).Verify(p => p(), Times.Once);
        }

        [Test, CustomAutoData]
        public void Create_returns_null_if_no_specification_is_added()
        {
            var sut = new CustomizableBindingFactory(Array.Empty<IBindingFactoryCustomization>());

            var binding = sut.Create(typeof(ITestService));

            Assert.That(binding, Is.Null);
        }
    }
}