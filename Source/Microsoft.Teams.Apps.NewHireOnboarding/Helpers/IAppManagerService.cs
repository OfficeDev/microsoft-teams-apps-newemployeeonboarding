// <copyright file="IAppManagerService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Helpers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Manage Teams Apps for a user.
    /// </summary>
    public interface IAppManagerService
    {
        /// <summary>
        /// Installs App for a user.
        /// </summary>
        /// <param name="token">Application access token.</param>
        /// <param name="appId">Teams App Id.</param>
        /// <param name="userIds">List of user's AAD Id.</param>
        /// <returns>A <see cref="Task"/>Representing the asynchronous operation.</returns>
        public Task InstallAppForUserAsync(string token, string appId, List<string> userIds);
    }
}
