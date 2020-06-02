using System;
using System.ServiceModel;
using System.Xml;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using EMG.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Moq;

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

            fixture.Customize<Uri>(o => o.FromFactory((string host, int port, string path) =>
            {
                var builder = new UriBuilder(Uri.UriSchemeNetTcp, host, port, path);
                return builder.Uri;
            }));

            fixture.Customize<ServiceCollection>(o => o.Do(services => { services.AddLogging(); }));

            fixture.Customize<IDiscoveryAdapter>(o => o.FromFactory((Uri seed) =>
            {
                var mock = new Mock<IDiscoveryAdapter>();
                mock.As<ICommunicationObject>();

                mock.Setup(p => p.Discover(It.IsAny<XmlQualifiedName>())).ReturnsUsingFixture(fixture);

                return mock.Object;
            }));

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