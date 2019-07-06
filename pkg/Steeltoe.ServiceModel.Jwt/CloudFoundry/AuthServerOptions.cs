﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Steeltoe.Security.Authentication.CloudFoundry
{
  public class AuthServerOptions
  {
    /// <summary>
    /// Gets or sets the location of the OAuth server
    /// </summary>
    public string AuthorizationUrl { get; set; }

    /// <summary>
    /// Gets or sets the location the user is sent to after authentication
    /// </summary>
    public string CallbackUrl { get; set; }

    /// <summary>
    /// Gets or sets the application's client id for interacting with the auth server
    /// </summary>
    public string ClientId { get; set; } = CloudFoundryDefaults.ClientId;

    /// <summary>
    /// Gets or sets the application's client secret for interacting with the auth server
    /// </summary>
    public string ClientSecret { get; set; } = CloudFoundryDefaults.ClientSecret;

    /// <summary>
    /// Gets or sets the name of the authentication type currently in use
    /// </summary>
    public string SignInAsAuthenticationType { get; set; }

    /// <summary>
    /// Gets or sets the timeout (in ms) for calls to the auth server
    /// </summary>
    public int ClientTimeout { get; set; } = 3000;

    /// <summary>
    /// Gets or sets additional scopes beyond 'openid' when requesting tokens
    /// </summary>
    public string AdditionalTokenScopes { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a scopes to require
    /// </summary>
    public string[] RequiredScopes { get; set; }

    /// <summary>
    /// Gets or sets a list of additional audiences to use with token validation
    /// </summary>
    public string[] AdditionalAudiences { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to validate SSO server certificate
    /// </summary>
    public bool ValidateCertificates { get; set; } = true;
  }
}
