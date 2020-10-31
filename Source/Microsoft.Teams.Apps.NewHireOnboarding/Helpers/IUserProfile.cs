// <copyright file="IUserProfile.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Helpers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Graph;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.SharePoint;

    /// <summary>
    /// Interface to provide the helper methods to access user profiles from Microsoft Graph API.
    /// </summary>
    public interface IUserProfile
    {
        /// <summary>
        /// Get user profile details from Microsoft Graph API.
        /// </summary>
        /// <param name="token">Microsoft Graph user access token.</param>
        /// <param name="userIds">Aad object id of users.</param>
        /// <returns>User profile details.</returns>
        Task<List<User>> GetUserProfileAsync(string token, List<string> userIds);

        /// <summary>
        /// Get user photo from Microsoft Graph.
        /// </summary>
        /// <param name="token">Microsoft Graph user access token.</param>
        /// <param name="userId">Aad object id of user.</param>
        /// <returns>User photo details.</returns>
        Task<Stream> GetUserPhotoAsync(string token, string userId);

        /// <summary>
        /// Get user profile notes from Microsoft Graph.
        /// </summary>
        /// <param name="token">Microsoft Graph user access token.</param>
        /// <param name="userId">Aad object id of user.</param>
        /// <returns>User profile note.</returns>
        Task<string> GetUserProfileNoteAsync(string token, string userId);

        /// <summary>
        /// Get user manager from Microsoft Graph.
        /// </summary>
        /// <param name="token">Microsoft Graph user access token.</param>
        /// <returns>User manager profile details.</returns>
        Task<UserProfileDetail> GetMyManagerAsync(string token);

        /// <summary>
        /// Get manager ids for a given list of user Ids from Microsoft Graph.
        /// </summary>
        /// <param name="token">Microsoft Graph application access token.</param>
        /// <param name="userIds">List of user Ids.</param>
        /// <returns>Returns list of manager Ids.</returns>
        Task<IEnumerable<string>> GetUserManagerIdsAsync(string token, IEnumerable<string> userIds);
    }
}
