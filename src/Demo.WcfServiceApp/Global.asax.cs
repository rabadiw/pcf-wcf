using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Configuration;

namespace Demo.WcfServiceApp
{
  public class Global : System.Web.HttpApplication
  {
    internal string AuthTokenUrl { get; } = ConfigurationManager.AppSettings["oauth_token_url"];
    internal string ClientId { get; } = ConfigurationManager.AppSettings["oauth_client_id"];
    internal string ClientSecret { get; } = ConfigurationManager.AppSettings["oauth_client_secret"];
    internal string AuthAudience { get; } = ConfigurationManager.AppSettings["oauth_audience"];

    protected void Application_Start(object sender, EventArgs e)
    {
      // options is static, set it on startup 
      var options = new Steeltoe.Security.Authentication.CloudFoundry.Wcf.CloudFoundryOptions();
      options.AuthorizationUrl = AuthTokenUrl;
      options.ClientId = ClientId;
      options.ClientSecret = ClientSecret;
      options.AdditionalAudiences = AuthAudience.Split(';');
      options.Initialize();

      new Steeltoe.Security.Authentication.CloudFoundry.Wcf.JwtAuthorizationManager(options);
    }

    protected void Session_Start(object sender, EventArgs e)
    {

    }

    protected void Application_BeginRequest(object sender, EventArgs e)
    {

    }

    protected void Application_AuthenticateRequest(object sender, EventArgs e)
    {

    }

    protected void Application_Error(object sender, EventArgs e)
    {
      // Global error capture
      Exception lastError = Server.GetLastError();
      Console.WriteLine("Unhandled exception: " + lastError.Message + lastError.StackTrace);
    }

    protected void Session_End(object sender, EventArgs e)
    {

    }

    protected void Application_End(object sender, EventArgs e)
    {

    }
  }
}