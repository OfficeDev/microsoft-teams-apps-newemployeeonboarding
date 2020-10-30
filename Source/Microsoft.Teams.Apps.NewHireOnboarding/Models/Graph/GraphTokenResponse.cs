// <copyright file="GraphTokenResponse.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models.Graph
{
    using Newtonsoft.Json;

    /// <summary>
    /// Model for the Microsoft Graph token response.
    /// </summary>
    public class GraphTokenResponse
    {
        /// <summary>
        /// Gets or sets the token_type.
        /// </summary>
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// Gets or sets the duration as to when the token will expire.
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the access_token.
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
