using System.ServiceModel;
using System.ServiceModel.Channels;

namespace EMG.Extensions.DependencyInjection.Discovery
{
    public interface IChannelFactoryWrapper
    {
        TChannel CreateChannel<TChannel>(Binding binding, EndpointAddress address) where TChannel: class;
    }

    public class ChannelFactoryWrapper : IChannelFactoryWrapper
    {
        public TChannel CreateChannel<TChannel>(Binding binding, EndpointAddress address) where TChannel : class
        {
            return ChannelFactory<TChannel>.CreateChannel(binding, address);
        }
    }
}