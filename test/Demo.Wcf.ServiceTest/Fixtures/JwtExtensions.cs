using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Demo.WcfServiceTest.Fixtures
{
  internal static class JwtExtensions
  {
    public static async Task<string> GetAccessToken()
    {
      var authTokenUrl = new Uri(new Uri(ConfigurationManager.AppSettings["oauth_token_url"]), "/oauth/token");
      // Aka AppId
      var clientId = ConfigurationManager.AppSettings["oauth_client_id"];
      // a.k.a AppSecret
      var clientSecret = ConfigurationManager.AppSettings["oauth_client_secret"];
      var oauthAudience = ConfigurationManager.AppSettings["oauth_audience"];

      var body = new Dictionary<string, string>
      {
        { "grant_type", "client_credentials" },
        { "client_id" , clientId },
        { "client_secret" , clientSecret },
        { "scope" , oauthAudience }
      };

      using (var client = new HttpClient())
      using (var content = new FormUrlEncodedContent(body))
      {
        content.Headers.Clear();
        content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

        HttpResponseMessage response = await client.PostAsync(authTokenUrl, content);

        var jwt = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
        return jwt.GetValue("access_token").ToString();
      }
    }

    public static T SyncResult<T>(this Task<T> taskToRun)
    {
      var predicate = Task.Run(async () => await taskToRun);
      Task.WaitAll(predicate);
      return predicate.Result;
    }
  }
}
