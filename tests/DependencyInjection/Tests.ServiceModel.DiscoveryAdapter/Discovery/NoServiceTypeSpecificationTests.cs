using System;
using EMG.Extensions.DependencyInjection.Discovery;
using NUnit.Framework;

namespace Tests.Discovery 
{
    [TestFixture]
    public class NoServiceTypeSpecificationTests 
    {
        [Test, CustomAutoData]
        public void IsSatisfiedBy_returns_true_if_type_is_null(NoServiceTypeSpecification sut)
        {
            var result = sut.IsSatisfiedBy(null);

            Assert.That(result, Is.True);
        }

        [Test, CustomAutoData]
        public void IsSatisfiedBy_returns_false_for_any_type(NoServiceTypeSpecification sut, Type serviceType)
        {
            var result = sut.IsSatisfiedBy(serviceType);

            Assert.That(result, Is.False);
        }
    }
}