using System;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Extensions.DependencyInjection.Discovery;
using NUnit.Framework;

namespace Tests.Discovery 
{
    [TestFixture]
    public class ServiceTypeSpecificationTests
    {
        [Test, CustomAutoData]
        public void Constructor_does_not_accept_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ServiceTypeSpecification).GetConstructors());
        }

        [Test, CustomAutoData]
        public void IsSatisfiedBy_returns_true_if_type_matches([Frozen] Type serviceType, ServiceTypeSpecification sut)
        {
            var result = sut.IsSatisfiedBy(serviceType);

            Assert.That(result, Is.True);
        }

        [Test, CustomAutoData]
        public void IsSatisfiedBy_returns_false_if_type_does_not_match([Frozen] Type serviceType, ServiceTypeSpecification sut, Type anotherServiceType)
        {
            Assume.That(serviceType, Is.Not.EqualTo(anotherServiceType));

            var result = sut.IsSatisfiedBy(serviceType);

            Assert.That(result, Is.False);
        }

        [Test, CustomAutoData]
        public void IsSatisfiedBy_returns_false_if_type_is_null(ServiceTypeSpecification sut)
        {
            var result = sut.IsSatisfiedBy(null);

            Assert.That(result, Is.False);
        }
    }
}