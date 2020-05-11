using System;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddServiceDiscoveryAdapter(configuration.GetSection("Discovery"),
                o =>
                {
                    o.ConfigureDiscoveryAdapterBinding = binding =>
                    {
                        binding.Security = new NetTcpSecurity
                        {
                            Mode = SecurityMode.None
                        };
                    };
                });

            services.DiscoverServiceUsingAdapter<IMyService>(binding => binding.Security.Mode = SecurityMode.None);

            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetService<IMyService>();

            Console.WriteLine($"Service { (service != null ? "" : "not ")}found");
        }
    }

    [ServiceContract]
    public interface IMyService
    {
        [OperationContract]
        string Echo(string message);
    }
}
