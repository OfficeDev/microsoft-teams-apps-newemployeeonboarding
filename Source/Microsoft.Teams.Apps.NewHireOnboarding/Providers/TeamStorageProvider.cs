// <copyright file="TeamStorageProvider.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Configuration;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Implements the methods that are defined in <see cref="ITeamStorageProvider"/>.
    /// Implements storage provider which helps to storage team information in Azure Table Storage.
    /// </summary>
    public class TeamStorageProvider : BaseStorageProvider, ITeamStorageProvider
    {
        private const string TeamConfigurationTable = "TeamConfiguration";

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamStorageProvider"/> class.
        /// </summary>
        /// <param name="options">A set of key/value application configuration properties for Azure Table Storage.</param>
        /// <param name="logger">Logger implementation to send logs to the logger service.</param>
        public TeamStorageProvider(
            IOptions<StorageSettings> options,
            ILogger<TeamStorageProvider> logger)
            : base(options?.Value.ConnectionString, TeamConfigurationTable, logger)
        {
        }

        /// <summary>
        /// Store or update team detail in Azure Table Storage.
        /// </summary>
        /// <param name="teamEntity">Represents team entity used for storage and retrieval.</param>
        /// <returns><see cref="Task"/> that represents team entity is saved or updated.</returns>
        public async Task<bool> StoreOrUpdateTeamDetailAsync(TeamEntity teamEntity)
        {
            teamEntity = teamEntity ?? throw new ArgumentNullException(nameof(teamEntity));

            if (string.IsNullOrWhiteSpace(teamEntity.ServiceUrl) || string.IsNullOrWhiteSpace(teamEntity.TeamId))
            {
                return false;
            }

            var result = await this.InsertOrReplaceTeamsAsync(teamEntity);

            if (result == null)
            {
                return false;
            }
            else
            {
                return result.HttpStatusCode == (int)HttpStatusCode.NoContent;
            }
        }

        /// <summary>
        /// Get team detail from Azure Table Storage.
        /// </summary>
        /// <param name="teamId">Team id.</param>
        /// <returns><see cref="Task"/>Already saved team details.</returns>
        public async Task<TeamEntity> GetTeamDetailAsync(string teamId)
        {
            if (string.IsNullOrWhiteSpace(teamId))
            {
                return null;
            }

            await this.EnsureInitializedAsync();

            var operation = TableOperation.Retrieve<TeamEntity>(teamId, teamId);
            var data = await this.CloudTable.ExecuteAsync(operation);

            return data.Result as TeamEntity;
        }

        /// <summary>
        /// Get all team details from Azure Table Storage.
        /// </summary>
        /// <returns><see cref="Task"/> List of team details.</returns>
        public async Task<List<TeamEntity>> GetAllTeamDetailAsync()
        {
            await this.EnsureInitializedAsync();

            var data = await this.CloudTable.ExecuteQuerySegmentedAsync(new TableQuery<TeamEntity>(), null);

            return data.Results as List<TeamEntity>;
        }

        /// <summary>
        /// This method delete the team detail record from table.
        /// </summary>
        /// <param name="teamEntity">Team configuration table entity.</param>
        /// <returns>A <see cref="Task"/> of type bool where true represents entity record is successfully deleted from table while false indicates failure in deleting data.</returns>
        public async Task<bool> DeleteTeamDetailAsync(TeamEntity teamEntity)
        {
            teamEntity = teamEntity ?? throw new ArgumentNullException(nameof(teamEntity));

            await this.EnsureInitializedAsync();

            TableOperation insertOrMergeOperation = TableOperation.Delete(teamEntity);
            TableResult result = await this.CloudTable.ExecuteAsync(insertOrMergeOperation);

            return result.HttpStatusCode == (int)HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Stores or update team details data in Azure Table Storage.
        /// </summary>
        /// <param name="entity">Holds team idea detail entity data.</param>
        /// <returns>A task that represents team entity data is saved or updated.</returns>
        private async Task<TableResult> InsertOrReplaceTeamsAsync(TeamEntity entity)
        {
            await this.EnsureInitializedAsync();

            TableOperation addOrUpdateOperation = TableOperation.InsertOrReplace(entity);

            return await this.CloudTable.ExecuteAsync(addOrUpdateOperation);
        }
    }
}
