// <copyright file="TokenHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Helpers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Configuration;

    /// <summary>
    /// Implements the methods that are defined in <see cref="ITokenHelper"/>.
    /// Helper class for token generation, validation and generate Azure Active Directory user access token for given resource, e.g. Microsoft Graph, SharePoint.
    /// </summary>
    public class TokenHelper : ITokenHelper
    {
        /// <summary>
        /// Microsoft Graph API base url.
        /// </summary>
        private const string GraphAPIBaseURL = "https://graph.microsoft.com/";

        /// <summary>
        /// Instance of the Microsoft Bot Connector OAuthClient class.
        /// </summary>
        private readonly OAuthClient oAuthClient;

        /// <summary>
        /// AADv1 bot connection name.
        /// </summary>
        private readonly string connectionName;

        /// <summary>
        /// Represents a set of key/value application configuration properties related to custom token.
        /// </summary>
        private readonly TokenSettings options;

        /// <summary>
        /// Sends logs to the Application Insights service.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenHelper"/> class.
        /// Helps generating custom token, validating custom token and generate AADv1 user access token for given resource.
        /// </summary>
        /// <param name="oAuthClient">Instance of the Microsoft Bot Connector OAuthClient class.</param>
        /// <param name="optionsAccessor">A set of key/value application configuration properties jwt access token.</param>
        /// <param name="logger">Instance to send logs to the Application Insights service.</param>
        public TokenHelper(OAuthClient oAuthClient, IOptions<TokenSettings> optionsAccessor, ILogger<TokenHelper> logger)
        {
            optionsAccessor = optionsAccessor ?? throw new ArgumentNullException(nameof(optionsAccessor));

            this.options = optionsAccessor.Value ?? throw new InvalidOperationException(nameof(optionsAccessor.Value));
            this.oAuthClient = oAuthClient ?? throw new ArgumentNullException(nameof(oAuthClient));
            this.connectionName = this.options.ConnectionName ?? throw new InvalidOperationException(nameof(this.options.ConnectionName));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get user access token for given resource using Bot OAuth client instance.
        /// </summary>
        /// <param name="fromId">Activity from id.</param>
        /// <returns>A task that represents security access token for given resource.</returns>
        public async Task<string> GetUserTokenAsync(string fromId)
        {
            try
            {
                var token = await this.oAuthClient.UserToken.GetAadTokensAsync(fromId, this.connectionName, new Microsoft.Bot.Schema.AadResourceUrls { ResourceUrls = new string[] { GraphAPIBaseURL } }).ConfigureAwait(false);
                return token?[GraphAPIBaseURL]?.Token;
            }
#pragma warning disable CA1031 // Catching general exception for any errors occurred during get user AAD access token.
            catch (Exception ex)
#pragma warning restore CA1031 // Catching general exception for any errors occurred during get user AAD access token.
            {
                this.logger.LogError(ex, "Failed to get user AAD access token for given resource using Bot OAuth client instance.");
                return null;
            }
        }
    }
}
