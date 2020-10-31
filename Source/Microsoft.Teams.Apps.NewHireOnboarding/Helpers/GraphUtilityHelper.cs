// <copyright file="GraphUtilityHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Helpers
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Graph;
    using Newtonsoft.Json;

    /// <summary>
    /// Implements the methods that are defined in <see cref="IGraphUtilityHelper"/>.
    /// </summary>
    public class GraphUtilityHelper : IGraphUtilityHelper
    {
        /// <summary>
        /// Login request base URL.
        /// </summary>
        private const string LoginRequestBaseUrl = "https://login.microsoftonline.com";

        /// <summary>
        /// Microsoft Graph API base url.
        /// </summary>
        private const string GraphAPIBaseURL = "https://graph.microsoft.com/";

        /// <summary>
        /// Provides a base class for sending HTTP requests and receiving HTTP responses from a resource identified by a URI.
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// Instance to log details in application insights.
        /// </summary>
        private readonly ILogger<GraphUtilityHelper> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphUtilityHelper"/> class.
        /// </summary>
        /// <param name="httpClient">Instance of HttpClient</param>
        /// <param name="logger">Instance of ILogger</param>
        public GraphUtilityHelper(
            HttpClient httpClient,
            ILogger<GraphUtilityHelper> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IGraphUtilityHelper"/> class.
        /// Gets the application token.
        /// </summary>
        /// <param name="tenantId">Unique id of tenant.</param>
        /// <param name="clientId">The application client id.</param>
        /// <param name="clientSecret">The application client secret.</param>
        /// <returns>The application token.</returns>
        public async Task<GraphTokenResponse> ObtainApplicationTokenAsync(string tenantId, string clientId, string clientSecret)
        {
            var requestUrl = $"{LoginRequestBaseUrl}/{tenantId}/oauth2/v2.0/token";
            var stringQuery = $"client_id={clientId}&scope={Uri.EscapeDataString($"{GraphAPIBaseURL}/.default")}&client_secret={Uri.EscapeDataString(clientSecret)}&grant_type=client_credentials";

            using (var httpContent = new StringContent(stringQuery, Encoding.UTF8, "application/x-www-form-urlencoded"))
            {
                var response = await this.httpClient.PostAsync(new Uri(requestUrl), httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var graphTokenResponse = JsonConvert.DeserializeObject<GraphTokenResponse>(responseContent);
                    this.logger.LogInformation($"Token received: {graphTokenResponse.AccessToken}");

                    return graphTokenResponse;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Method to get HTTP response from Graph API.
        /// </summary>
        /// <param name="token">Graph API application access token.</param>
        /// <param name="requestPath">Graph API request URL.</param>
        /// <returns>A task that represents a HTTP response message including the status code and data.</returns>
        public async Task<HttpResponseMessage> GetAsync(string token, string requestPath)
        {
            HttpMethod httpMethod = new HttpMethod("GET");
            using (var request = new HttpRequestMessage(httpMethod, requestPath))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                return await this.httpClient.SendAsync(request);
            }
        }
    }
}
