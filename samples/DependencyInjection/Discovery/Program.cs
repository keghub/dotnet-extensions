using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using EMG.Extensions.DependencyInjection.Discovery.BindingCustomizations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Discovery
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new Dictionary<string, string>
            {
                ["Discovery:ProbeEndpoint"] = "net.tcp://localhost:8001/Probe"
            };

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(settings);

            var configuration = configurationBuilder.Build();

            var services = new ServiceCollection();

            services.AddLogging(l => l.AddConsole());

            services.AddServiceDiscovery()
                    .ConfigureServiceDiscovery(configuration.GetSection("Discovery"))
                    .ConfigureServiceDiscovery(o => o.DiscoveryBindingFactory = () => new NetTcpBinding(SecurityMode.None));

            // Customizes the binding used for net.tcp endpoints for the IMyService service
            //services.AddServiceBindingCustomization<IMyService>(Uri.UriSchemeNetTcp, () => new NetTcpBinding(SecurityMode.None));

            // Customizes the binding used for net.tcp endpoints for all services
            services.AddBindingCustomization<UnsafeNetTcpBindingFactoryCustomization>();

            services.DiscoverService<IMyService>();

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

    [ServiceContract(Namespace="http://samples.educations.com/", Name="MyService")]
    public interface IMyService
    {
        [OperationContract]
        string Echo(string message);
    }

    public class UnsafeNetTcpBindingFactoryCustomization : IBindingFactoryCustomization
    {
        public Type ServiceType { get; } = null;

        public string UriScheme { get; } = null;

        public Binding Create() => new NetTcpBinding(SecurityMode.None);
    }
}
