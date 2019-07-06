using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Demo.WcfServiceLibrary
{
  public sealed class GenericProxy<TContract> : IDisposable where TContract : class
  {
    private ChannelFactory<TContract> _channelFactory;
    private TContract _channel;

    public GenericProxy()
    {
      _channelFactory = new ChannelFactory<TContract>();
    }

    public GenericProxy(Binding binding, EndpointAddress remoteAddress, System.ServiceModel.Description.IEndpointBehavior endpointBehavior = null)
    {
      _channelFactory = new ChannelFactory<TContract>(binding, remoteAddress);
      if (endpointBehavior != null)
      {
        _channelFactory.Endpoint.EndpointBehaviors.Add(endpointBehavior);
      }
    }

    public void Execute(Action<TContract> action)
    {
      action.Invoke(Channel);
    }

    public TResult Execute<TResult>(Func<TContract, TResult> function)
    {
      return function.Invoke(Channel);
    }

    private TContract Channel
    {
      get
      {
        if (_channel == null)
        {
          _channel = _channelFactory.CreateChannel();
        }

        return _channel;
      }
    }

    public void Dispose()
    {
      try
      {
        if (_channel != null)
        {
          var currentChannel = _channel as IClientChannel;
          if (currentChannel.State == CommunicationState.Faulted)
          {
            currentChannel.Abort();
          }
          else
          {
            currentChannel.Close();
          }
        }
      }
      finally
      {
        _channel = null;
        GC.SuppressFinalize(this);
      }
    }
  }
}
