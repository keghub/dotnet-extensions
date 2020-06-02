using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Extensions.DependencyInjection.Discovery.BindingCustomizations;
using Moq;
using NUnit.Framework;

namespace Tests.Discovery
{
    [TestFixture]
    public class ServiceBindingFactoryCustomizationTests
    {
        [Test]
        [CustomAutoData]
        public void BindingFactory_is_used([Frozen] Func<Binding> bindingFactory, ServiceBindingFactoryCustomization<ITestService> sut)
        {
            _ = sut.Create();

            Mock.Get(bindingFactory as Func<BasicHttpBinding>).Verify(p => p(), Times.Once);
        }

        [Test]
        [CustomAutoData]
        public void Constructor_does_not_accept_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ServiceBindingFactoryCustomization<>).GetConstructors());
        }

        [Test]
        [CustomAutoData]
        public void ServiceType_is_exposed(ServiceBindingFactoryCustomization<ITestService> sut)
        {
            Assert.That(sut.ServiceType, Is.EqualTo(typeof(ITestService)));
        }

        [Test]
        [CustomAutoData]
        public void UriScheme_is_exposed([Frozen] string uriScheme, ServiceBindingFactoryCustomization<ITestService> sut)
        {
            Assert.That(sut.UriScheme, Is.EqualTo(uriScheme));
        }
    }
}