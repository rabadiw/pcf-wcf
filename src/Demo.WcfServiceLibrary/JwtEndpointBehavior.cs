namespace Demo.WcfServiceLibrary
{
  using System;
  using System.ServiceModel;
  using System.ServiceModel.Channels;
  using System.ServiceModel.Description;
  using System.ServiceModel.Dispatcher;
  using System.Xml;

  public class JwtEndpointBehavior : IEndpointBehavior, IClientMessageInspector
  {
    private string token;

    public JwtEndpointBehavior(string token)
    {
      this.token = token;
    }

    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    {
    }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
    {
      clientRuntime.MessageInspectors.Add(this);
    }

    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
    {
    }

    public void Validate(ServiceEndpoint endpoint)
    {
    }

    public void AfterReceiveReply(ref Message reply, object correlationState)
    {
    }

    public object BeforeSendRequest(ref Message request, IClientChannel channel)
    {
      HttpRequestMessageProperty header = default;

      // Use existing
      if (request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
      {
        header = request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
      }
      // Otherwise, create new one
      if (header == null)
      {
        header = new HttpRequestMessageProperty();
        // Add it to the request
        request.Properties.Add(HttpRequestMessageProperty.Name, header);
      }

      // Set Bearer header
      header.Headers.Add("Authorization", "Bearer " + this.token);

      return null;
    }
  }
}