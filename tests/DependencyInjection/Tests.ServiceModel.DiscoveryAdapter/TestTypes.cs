using System.ServiceModel;

namespace Tests
{
    [ServiceContract(Namespace = "http://samples.educations.com/", Name = "TestService")]
    public interface ITestService
    {
        [OperationContract]
        string Echo(string message);
    }
}