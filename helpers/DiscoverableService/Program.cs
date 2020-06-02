using System;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using EMG.Utilities.ServiceModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EndpointAddress = EMG.Utilities.ServiceModel.Configuration.EndpointAddress;

namespace DiscoverableService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddOptions();

            services.AddWcfService<MyTestService>(service =>
            {
                service.AddNetTcpEndpoint(typeof(IMyService), EndpointAddress.ForNetTcp(11000), binding => binding.WithNoSecurity()).Discoverable();
            });

            services.AddDiscovery<NetTcpBinding>(new Uri("net.tcp://localhost:8001/Announcement"), TimeSpan.FromSeconds(10), b => b.WithNoSecurity());

            var sp = services.BuildServiceProvider();

            var hostedService = sp.GetRequiredService<IHostedService>();

            await hostedService.StartAsync(CancellationToken.None);

            Console.WriteLine("Service started!");

            Console.ReadLine();

            await hostedService.StopAsync(CancellationToken.None);
        }
    }

    [ServiceContract(Namespace = "http://samples.educations.com/", Name = "MyService")]
    public interface IMyService
    {
        [OperationContract]
        string Echo(string message);
    }

    public class MyTestService : IMyService
    {
        public string Echo(string message)
        {
            return message;
        }
    }
}
