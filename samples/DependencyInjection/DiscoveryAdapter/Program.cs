using System;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscoveryAdapter
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new Dictionary<string, string>
            {
                ["Discovery:ProbeEndpoint"] = "net.tcp://localhost:8001/Probe-Adapter"
            };

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(settings);

            var configuration = configurationBuilder.Build();

            var services = new ServiceCollection();

            services.AddLogging(l => l.AddConsole());

            services.AddServiceDiscoveryAdapter()
                    .ConfigureServiceDiscovery(configuration.GetSection("Discovery"))
                    .ConfigureServiceDiscovery(o =>
            {
                o.ConfigureDiscoveryAdapterBinding = binding =>
                {
                    binding.Security.Mode = SecurityMode.None;
                };
            });

            services.AddBindingCustomization(binding => binding.Security.Mode = SecurityMode.None);

            services.DiscoverServiceUsingAdapter<IMyService>();

            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetService<IMyService>();

            var isServiceFound = service != null;

            Console.WriteLine($"Service { (isServiceFound ? "" : "not ")}found");

            if (isServiceFound)
            {
                var response = service.Echo("This is my message");

                Console.WriteLine(response);
            }
        }
    }

    [ServiceContract(Namespace = "http://samples.educations.com/", Name = "MyService")]
    public interface IMyService
    {
        [OperationContract]
        string Echo(string message);
    }
}
