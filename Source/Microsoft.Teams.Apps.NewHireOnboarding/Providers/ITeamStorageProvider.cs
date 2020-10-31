// <copyright file="ITeamStorageProvider.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Providers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;

    /// <summary>
    /// Interface for team storage provider.
    /// </summary>
    public interface ITeamStorageProvider
    {
        /// <summary>
        /// Store or update team details in the storage.
        /// </summary>
        /// <param name="teamEntity">Represents team entity used for storage and retrieval.</param>
        /// <returns><see cref="Task"/> Returns the status whether team entity is stored or not.</returns>
        Task<bool> StoreOrUpdateTeamDetailAsync(TeamEntity teamEntity);

        /// <summary>
        ///  Get team detail from Azure Table Storage.
        /// </summary>
        /// <param name="teamId">Team id.</param>
        /// <returns><see cref="Task"/>Already saved team details.</returns>
        Task<TeamEntity> GetTeamDetailAsync(string teamId);

        /// <summary>
        /// This method delete the team detail record from the storage.
        /// </summary>
        /// <param name="teamEntity">Team configuration entity.</param>
        /// <returns>A <see cref="Task"/> of type bool where true represents entity record is successfully deleted from the storage while false indicates failure in deleting data.</returns>
        Task<bool> DeleteTeamDetailAsync(TeamEntity teamEntity);

        /// <summary>
        /// Get all team details from Azure Table Storage.
        /// </summary>
        /// <returns><see cref="Task"/> List of team details.</returns>
        Task<List<TeamEntity>> GetAllTeamDetailAsync();
    }
}
