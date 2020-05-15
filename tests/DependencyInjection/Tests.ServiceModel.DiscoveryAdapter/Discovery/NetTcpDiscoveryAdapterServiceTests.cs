using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Text;
using AutoFixture.Idioms;
using EMG.Extensions.DependencyInjection.Discovery;
using NUnit.Framework;

namespace Tests.Discovery
{
    [TestFixture]
    public class NetTcpDiscoveryAdapterServiceTests
    {
        [Test, CustomAutoData]
        public void Constructor_does_not_accept_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(NetTcpDiscoveryAdapterService).GetConstructors());
        }
    }
}
