// <copyright file="ITeamMembership.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Helpers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Graph;

    /// <summary>
    /// Interface to provide the helper methods to access team operations from Microsoft Graph API.
    /// </summary>
    public interface ITeamMembership
    {
        /// <summary>
        /// Get joined teams from Microsoft Graph.
        /// </summary>
        /// <param name="token">Microsoft Graph application access token.</param>
        /// <returns>Joined teams details.</returns>
        Task<List<Team>> GetMyJoinedTeamsAsync(string token);

        /// <summary>
        /// Get joined teams from Microsoft Graph.
        /// </summary>
        /// <param name="token">Microsoft Graph application access token.</param>
        /// <param name="teamId">Unique id of Teams.</param>
        /// <returns>Channels details.</returns>
        Task<List<Channel>> GetChannelsAsync(string token, string teamId);

        /// <summary>
        /// Get group member Ids from Microsoft Graph.
        /// </summary>
        /// <param name="token">Microsoft Graph application access token.</param>
        /// <param name="groupId">Unique id of Azure Active Directory security group.</param>
        /// <returns>Group members.</returns>
        Task<List<string>> GetGroupMemberIdsAsync(string token, string groupId);
    }
}
