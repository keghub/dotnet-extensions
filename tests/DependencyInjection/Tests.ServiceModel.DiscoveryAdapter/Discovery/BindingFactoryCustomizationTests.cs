using System;
using System.ServiceModel;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Extensions.DependencyInjection.Discovery;
using Moq;
using NUnit.Framework;

namespace Tests.Discovery {
    [TestFixture]
    public class BindingFactoryCustomizationTests
    {
        [Test, CustomAutoData]
        public void Constructor_does_not_accept_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(BindingFactoryCustomization).GetConstructors());
        }

        [Test, CustomAutoData]
        public void ServiceTypeSpecification_is_exposed_from_property([Frozen] IServiceTypeSpecification specification, BindingFactoryCustomization sut)
        {
            Assert.That(sut.ServiceSpecification, Is.SameAs(specification));
        }

        [Test, CustomAutoData]
        public void Create_uses_BindingFactory([Frozen] Func<NetTcpBinding> bindingFactory, BindingFactoryCustomization sut)
        {
            _ = sut.Create();

            Mock.Get(bindingFactory).Verify(p => p(), Times.Once());
        }
    }
}