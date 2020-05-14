using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
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

            fixture.Customize<Func<Binding>>(o => o.FromFactory((Func<BasicHttpBinding> basicHttpBindingFactory) => basicHttpBindingFactory as Func<Binding>));

            fixture.Inject(BasicHttpSecurityMode.None);

            fixture.Customize<EndpointAddress>(o => o.FromFactory((Uri uri) => new EndpointAddress(uri.ToString())));

            return fixture;
        }
    }
}