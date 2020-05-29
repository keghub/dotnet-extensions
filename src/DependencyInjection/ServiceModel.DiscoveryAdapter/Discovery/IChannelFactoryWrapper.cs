using System.ServiceModel;
using System.ServiceModel.Channels;

namespace EMG.Extensions.DependencyInjection.Discovery 
{
    public interface IChannelFactoryWrapper
    {
        TChannel CreateChannel<TChannel>(Binding binding, EndpointAddress address);
    }

    public class ChannelFactoryWrapper : IChannelFactoryWrapper 
    {
        public TChannel CreateChannel<TChannel>(Binding binding, EndpointAddress address)
        {
            var channelFactory = new ChannelFactory<TChannel>(binding, address);
            return channelFactory.CreateChannel();
        }
    }
}