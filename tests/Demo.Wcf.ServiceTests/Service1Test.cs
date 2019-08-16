using System;
using Xunit;
using FluentAssertions;
using System.ServiceModel;
using Demo.WcfServiceTest.Fixtures;
using System.Net;

namespace Demo.WcfServiceTest
{
  public class Service1Test
  {
    static (string Local, string LocalTls, string Pcf) _hostlist = (
        @Local: "http://localhost:55952",
        @LocalTls: "https://localhost:44375",
        @Pcf: "https://wcf-ws-secure.apps.pcfone.io"
      );
    private readonly string Service1Uri = $"{_hostlist.Pcf}/Service1.svc";
    private readonly string Service2Uri = $"{_hostlist.Pcf}/Service2.svc";

    [Fact(DisplayName = "Service1.GetData Fail w/o security")]
    public void Fail_Service1_GetData()
    {
      var binding = new BasicHttpBinding();
      // Note: these 2 lines enable TLS
      binding.Security.Mode = BasicHttpSecurityMode.Transport;
      binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;

      var remoteAddress = new EndpointAddress(Service1Uri);

      using (var proxy = new WcfServiceLibrary.GenericProxy<WcfServiceLibrary.IService1>(binding, remoteAddress))
      {
        // Act
        Action actual = () => proxy.Execute(x => x.GetData(1));
        actual.Should().Throw<System.ServiceModel.Security.MessageSecurityException>()
            .Which.GetBaseException().As<WebException>()
            .Response.As<HttpWebResponse>()
            .StatusCode.Should().Be(HttpStatusCode.Unauthorized);
      }
    }

    [Fact(DisplayName = "Service2.GetSecureData Fail w/o access token")]
    public void Fail_Service2_GetSecureData()
    {
      var binding = new BasicHttpBinding();
      // Note: these 2 lines enable TLS
      binding.Security.Mode = BasicHttpSecurityMode.Transport;
      binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;

      var remoteAddress = new EndpointAddress(Service2Uri);

      using (var proxy = new WcfServiceLibrary.GenericProxy<WcfServiceLibrary.IService2>(binding, remoteAddress))
      {
        // Act
        Action actual = () => proxy.Execute(x => x.GetSecureData(1));
        actual.Should().Throw<System.ServiceModel.Security.MessageSecurityException>()
            .Which.GetBaseException().As<WebException>()
            .Response.As<HttpWebResponse>()
            .StatusCode.Should().Be(HttpStatusCode.Unauthorized);
      }
    }

    [Fact(DisplayName = "Service2.GetSecureData Fail w/o TLS")]
    public void Fail_Service2_GetSecureData_NoTLS()
    {
      var binding = new BasicHttpBinding();
      // Note: these 2 lines disable TLS
      binding.Security.Mode = BasicHttpSecurityMode.None;
      binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
      // Extend timeout
      binding.ReceiveTimeout = TimeSpan.FromMinutes(2);
      binding.SendTimeout = TimeSpan.FromMinutes(2);

      // Use HTTP scheme
      var remoteAddress = new EndpointAddress(Service2Uri.Replace("https://", "http://"));

      using (var proxy = new WcfServiceLibrary.GenericProxy<WcfServiceLibrary.IService2>(binding, remoteAddress))
      {
        // Act
        Action actual = () => proxy.Execute(x => x.GetSecureData(1));
        actual.Should().Throw<System.ServiceModel.CommunicationException>();
      }
    }

    [Fact(DisplayName = "Service2.GetSecureData Should w/TLS, AccessToken")]
    public void Should_Service2_GetSecureData()
    {
      var binding = new BasicHttpBinding();
      // Note: these 2 lines enable TLS
      binding.Security.Mode = BasicHttpSecurityMode.Transport;
      binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
      // Extend timeout
      binding.ReceiveTimeout = TimeSpan.FromMinutes(2);
      binding.SendTimeout = TimeSpan.FromMinutes(2);

      var remoteAddress = new EndpointAddress(Service2Uri);

      var accessToken = Fixtures.JwtExtensions.GetAccessToken().SyncResult();
      var jwtEndpointBehavior = new Demo.WcfServiceLibrary.JwtEndpointBehavior(accessToken);

      using (var proxy = new WcfServiceLibrary.GenericProxy<WcfServiceLibrary.IService2>(binding, remoteAddress, jwtEndpointBehavior))
      {
        var actual = proxy.Execute(x => x.GetSecureData(1));
        actual.Should().Be("You securely entered: 1");
      }
    }
  }
}
