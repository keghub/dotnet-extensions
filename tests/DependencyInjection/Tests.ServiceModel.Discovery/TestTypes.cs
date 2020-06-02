using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using EMG.Extensions.DependencyInjection.Discovery.BindingCustomizations;

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
        public Type ServiceType { get; } = null;

        public string UriScheme { get; } = null;

        public Binding Create()
        {
            throw new NotImplementedException();
        }
    }
}