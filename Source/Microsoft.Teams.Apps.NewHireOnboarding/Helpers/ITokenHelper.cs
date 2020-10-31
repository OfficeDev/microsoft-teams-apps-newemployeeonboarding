// <copyright file="ITokenHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Helpers
{
    using System.Threading.Tasks;

    /// <summary>
    /// This interface will contain the helper methods to generate Azure Active Directory user access token for given resource, e.g. Microsoft Graph.
    /// </summary>
    public interface ITokenHelper
    {
        /// <summary>
        /// Get user access token for given resource using Bot OAuth client instance.
        /// </summary>
        /// <param name="fromId">Activity from id.</param>
        /// <returns>A task that represents security access token for given resource.</returns>
        Task<string> GetUserTokenAsync(string fromId);
    }
}
