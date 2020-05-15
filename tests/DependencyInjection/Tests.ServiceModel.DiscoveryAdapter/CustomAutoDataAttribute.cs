using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using EMG.Extensions.DependencyInjection.Discovery;
using Microsoft.Extensions.DependencyInjection;

namespace Tests
{
    public class CustomAutoDataAttribute : AutoDataAttribute
    {
        public CustomAutoDataAttribute() : base (CreateFixture) {}

        private static IFixture CreateFixture()
        {
            var fixture = new Fixture();

            fixture.Customize(new AutoMoqCustomization
            {
                ConfigureMembers = true,
                GenerateDelegates = true
            });

            fixture.Customize<ServiceCollection>(o => o.Do(services => { services.AddLogging(); }));

            fixture.Customize<NetTcpDiscoveryOptions>(o => o.With(p => p.ProbeEndpoint, (Uri seed) => CreateNetTcpUri(seed)));

            return fixture;
        }

        private static Uri CreateNetTcpUri(Uri seed)
        {
            var builder = new UriBuilder(seed)
            {
                Scheme = Uri.UriSchemeNetTcp
            };

            return builder.Uri;
        }
    }
}