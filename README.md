The purpose of the repo is to demonstrate the following [WCF](https://docs.microsoft.com/en-us/dotnet/framework/wcf/whats-wcf) HowTos when running on [Pivotal Cloud Foundry](https://pivotal.io/platform). This repo also demonstrates the classes used by [Steeltoe for WCF](https://github.com/SteeltoeOSS/steeltoe/tree/master/src/Security/src/Authentication.CloudFoundryWcf) to add OAuth 2.0 service behavior. 

## Folder breakdown
<pre>
├───tools
├───test
│   └───Demo.Wcf.ServiceTest
│       └───Fixtures
├───pkg
│   └───Steeltoe.ServiceModel.Jwt
│       └───CloudFoundry
└───src
    ├───Demo.WcfServiceApp
    └───Demo.WcfServiceLibrary
</pre>

- **tools** - contains a powershell script that demonstrates how to retrieve an AccessToken. It reads the `OAuth 2.0` properties from file `oauth-test.json`. This file is not checked in and you are required to create and fill in the data. Here's a template of the expected properties. See [Setup PCF SSO Tile](#Setup-PCF-SSO-Tile) section for instructions.
```json
{
    "AppID": "00000000-0000-0000-0000-000000000000",
    "AppSecret": "00000000-0000-0000-0000-000000000000",
    "OAuthTokenUrl": "https://domain/oauth/token"
}
```
- **test** - contains test cases that demonstrate usage of the service once deployed. The test are self explanatory and consist of.
    ```csharp
    // Steps to get an access token
    Demo.WcfServiceTest.Fixtures.JwtExtensions.GetAccessToken()

    // Test a call without security, should fail
    // assumption is that all endpoints are secure
    [Fact(DisplayName = "Service1.GetData Fail w/o security")]
    // Test a call without TLS Offloading, should fail
    [Fact(DisplayName = "Service2.GetSecureData Fail w/o TLS")]
    // Test a call without security, should fail
    [Fact(DisplayName = "Service2.GetSecureData Fail w/o access token")]    
    // Test a call with TLS Offloading and AccessToken, should succeed
    [Fact(DisplayName = "Service2.GetSecureData Should w/TLS, AccessToken")]
    ```
    > **Note:** the apppSettings.config file is not checked in and will be required. See [template](#appSettings.config-template).

- **pkg** - contains all the classes from Steeltoe that makes the OAuth 2.0 service behavior happen.
- **src** - contains two projects; a WCF Service App that is `AspNetCompatibilityEnabled=true`, and a WCF Service Library that is shared with the the WCF Service App project and the WCF Service Test project. The two classes to take note of in this project is the `GenericProxy.cs` and `JwtEndpointBehavior.cs`. The client usage is demonstrated within the test class `Service2.GetSecureData Should w/TLS, AccessToken`.
  > **Note:** the apppSettings.config file is not checked in and will be required. See [template](#appSettings.config-template).

## appSettings.config template
```xml
<appSettings>
  <!--Set to the root URL-->
  <add key="oauth_token_url" value="https://domain"/>
  <add key="oauth_client_id" value="00000000-0000-0000-0000-000000000000"/>
  <add key="oauth_client_secret" value="00000000-0000-0000-0000-000000000000"/>
  <add key="oauth_audience" value="openid"/>
</appSettings>
```
## Setup PCF SSO Tile
For Service-to-Service applications, Pivotal Single Sign-On (SSO) supports the Client Credentials OAuth 2.0 grant type. See [PCF Service-to-Service App](https://docs.pivotal.io/p-identity/1-9/configure-apps/service-to-service-app.html). At the end of the setup, you will be presented with an option to download a `.csv` file that contains the `App ID` and `App Secret`. You can also regenerate the `App Secret` frm within `View Credentials` section of the SSO manage screen. Within the `View Credentials` screen you will also be able to find the `OAuth Token URL`.

## Secure a WCF Service transport behind TLS Offloading
"SSL offloading is the process of removing the SSL-based encryption from incoming traffic to relieve a web server of the processing burden of decrypting and/or encrypting traffic sent via SSL. The processing is offloaded to a separate device designed specifically for SSL acceleration or SSL termination." --[F5](https://www.f5.com/services/resources/glossary/ssl-offloading)

To secure a WCF service transport with TLS Offloading, use [useRequestHeadersForMetadataAddress](https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/file-schema/wcf/userequestheadersformetadataaddress). This behavior extension enables the retrieval of metadata address information from the request message headers. See, [WCF: SSL offloading in load balancer – a simple approach](https://blogs.msdn.microsoft.com/dsnotes/2016/05/14/wcf-ssl-offloading-in-load-balancer-a-simple-approach) for more detail explanation.
```xml
<system.serviceModel>
  <behaviors>
    <serviceBehaviors>
      <behavior>
        ...
        <!-- Enables the retrieval of metadata address information from the request message headers -->
        <useRequestHeadersForMetadataAddress />
        ...
      </behavior>
    </serviceBehaviors>
  </behaviors>
</system.serviceModel>
```

## Secure a WCF Service with JWT
Take note, the following sample uses the project refrence `Steeltoe.ServiceModel.Jwt` as the assembly. For production, use the latest official Steeltoe package.
```powershell
Install-Package Steeltoe.Security.Authentication.CloudFoundryWcf -Version 2.2.0
```
```xml
<system.serviceModel>
  <behaviors>
    <serviceBehaviors>
      <behavior>
        <!-- To avoid disclosing metadata information, set the values below to false before deployment -->
        <serviceMetadata httpGetEnabled="false" httpsGetEnabled="true" />
        <!-- To receive exception details in faults for debugging purposes, set the value below to true.  Set to false before deployment to avoid disclosing exception information -->
        <serviceDebug includeExceptionDetailInFaults="true" />
        <!-- Enables the retrieval of metadata address information from the request message headers -->
        <useRequestHeadersForMetadataAddress />
        <!-- Add the following configuration to the WCF system.serviceModel:behaviors:serviceBehavior:behavior element -->
        <serviceAuthorization principalPermissionMode="Custom" serviceAuthorizationManagerType="Steeltoe.Security.Authentication.CloudFoundry.Wcf.JwtAuthorizationManager, Steeltoe.ServiceModel.Jwt" />
      </behavior>
    </serviceBehaviors>
  </behaviors>
  <protocolMapping>
    <add binding="basicHttpsBinding" scheme="https" />
    <add binding="basicHttpBinding" scheme="http" />
  </protocolMapping>
  <serviceHostingEnvironment aspNetCompatibilityEnabled="true" multipleSiteBindingsEnabled="true" />
</system.serviceModel>
```

## Special case: Integrated Windows Authentication
As of this writting, the PCF platform does not start app containers within an AD joined host. The PCF SSO Tile can be configured with different Identity Providers; e.g. AD FS, AAD, etc. With the IdP in place and configured, the WCF Service can add the `serviceAuthorization` to secure the entire service with OAuth 2.0 and TLS Offloading. 

## Deploy
To deploy the `src\Demo.WcfServiceApp` app, use the following powershell script. Replace `PivotalSSO` with your SSO tile instance name.
```powershell
# build using secure profile
.\deploy.development.ps1 -build -configuration ReleaseSecure 
# deploy to PCF
.\deploy.development.ps1 -deploy -secure -ssoServiceName PivotalSSO

# or build and deploy with one liner
.\deploy.development.ps1 -build -configuration ReleaseSecure -deploy -secure -ssoServiceName PivotalSSO
 ```