// <copyright file="IGraphUtilityHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Helpers
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Graph;

    /// <summary>
    /// This interface will contain helper methods for Microsoft Graph.
    /// </summary>
    public interface IGraphUtilityHelper
    {
        /// <summary>
        /// Gets the application token.
        /// </summary>
        /// <param name="tenantId">Unique id of tenant.</param>
        /// <param name="clientId">The application client id.</param>
        /// <param name="clientSecret">The application client secret.</param>
        /// <returns>The application token.</returns>
        Task<GraphTokenResponse> ObtainApplicationTokenAsync(
            string tenantId,
            string clientId,
            string clientSecret);

        /// <summary>
        /// Method to get HTTP response from Graph API.
        /// </summary>
        /// <param name="token">Graph API application access token.</param>
        /// <param name="requestPath">Graph API request URL.</param>
        /// <returns>A task that represents a HTTP response message including the status code and data.</returns>
        Task<HttpResponseMessage> GetAsync(string token, string requestPath);
    }
}
