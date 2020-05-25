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
    public class BindingFactoryCustomizationTests
    {
        [Test]
        [CustomAutoData]
        public void BindingFactory_is_used([Frozen] Func<Binding> bindingFactory, BindingFactoryCustomization sut)
        {
            _ = sut.Create();

            Mock.Get(bindingFactory as Func<BasicHttpBinding>).Verify(p => p(), Times.Once);
        }

        [Test]
        [CustomAutoData]
        public void Constructor_does_not_accept_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(BindingFactoryCustomization).GetConstructors());
        }

        [Test]
        [CustomAutoData]
        public void ServiceType_is_null(BindingFactoryCustomization sut)
        {
            Assert.That(sut.ServiceType, Is.Null);
        }

        [Test]
        [CustomAutoData]
        public void UriScheme_is_exposed([Frozen] string uriScheme, BindingFactoryCustomization sut)
        {
            Assert.That(sut.UriScheme, Is.EqualTo(uriScheme));
        }
    }
}