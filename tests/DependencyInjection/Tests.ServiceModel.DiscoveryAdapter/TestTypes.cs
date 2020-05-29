using System;
using System.ServiceModel;
using EMG.Extensions.DependencyInjection.Discovery;

namespace Tests
{
    [ServiceContract(Namespace = "http://samples.educations.com/", Name = "TestService")]
    public interface ITestService
    {
        [OperationContract]
        string Echo(string message);
    }

    public class TestCustomization : IBindingFactoryCustomization
    {
        public IServiceTypeSpecification ServiceSpecification { get; set; }

        public NetTcpBinding Create()
        {
            throw new NotImplementedException();
        }
    }
}