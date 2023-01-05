using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using EMG.Extensions.Logging.Loggly;
using WorldDomination.Net.Http;

namespace Tests
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute() : base(() =>
        {
            var fixture = new Fixture();

            fixture.Customize(new AutoMoqCustomization
            {
                ConfigureMembers = true,
                GenerateDelegates = true
            });

            fixture.Customizations.Add(new ElementsBuilder<Encoding>(Encoding.GetEncodings().Select(e => e.GetEncoding())));

            fixture.Customize<HttpClient>(c => c.FromFactory((FakeHttpMessageHandler handler) => new HttpClient(handler)));

            fixture.Register<CookieContainer>(() => null);

            fixture.Customize<LogglyOptions>(o => o
                                                  .Without(p => p.LogglyHost)
                                                  .Without(p => p.LogglyScheme)
                                                  .Without(p => p.SerializerSettings)
                                                  .With(p => p.SuppressExceptions, false)
                                                  .With(p => p.ContentEncoding, Encoding.UTF8));

            fixture.Customize<LogglyMessage>(o => o.With(p => p.Data, (string data) => data));

            fixture.Customize<Error>(o => o.With(p => p.Data, (Dictionary<string, string> d) => d));

            return fixture;
        }) { }
    }
}
