// <copyright file="IntroductionStorageProvider.cs" company="Microsoft">
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
    using Microsoft.Teams.Apps.NewHireOnboarding.Models;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Configuration;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Implements the methods that are defined in <see cref="IIntroductionStorageProvider"/>.
    /// Implements storage provider which helps to storage introduction information in Azure Table Storage.
    /// </summary>
    public class IntroductionStorageProvider : BaseStorageProvider, IIntroductionStorageProvider
    {
        private const string IntroductionConfigurationTable = "NewHireIntroduction";

        /// <summary>
        /// Initializes a new instance of the <see cref="IntroductionStorageProvider"/> class.
        /// </summary>
        /// <param name="options">A set of key/value application configuration properties for Azure Table Storage.</param>
        /// <param name="logger">Logger implementation to send logs to the logger service.</param>
        public IntroductionStorageProvider(
            IOptions<StorageSettings> options,
            ILogger<IntroductionStorageProvider> logger)
            : base(options?.Value.ConnectionString, IntroductionConfigurationTable, logger)
        {
        }

        /// <summary>
        /// Store or update new hire introduction detail in Azure Table Storage.
        /// </summary>
        /// <param name="introductionEntity">Represents new hire introduction entity used for storage and retrieval.</param>
        /// <returns><see cref="Task"/> that represents new hire introduction entity is saved or updated.</returns>
        public async Task<bool> StoreOrUpdateIntroductionDetailAsync(IntroductionEntity introductionEntity)
        {
            introductionEntity = introductionEntity ?? throw new ArgumentNullException(nameof(introductionEntity));

            if (string.IsNullOrWhiteSpace(introductionEntity.NewHireAadObjectId)
                || string.IsNullOrWhiteSpace(introductionEntity.ManagerAadObjectId)
                || string.IsNullOrWhiteSpace(introductionEntity.NewHireConversationId)
                || string.IsNullOrWhiteSpace(introductionEntity.ManagerConversationId)
                || string.IsNullOrWhiteSpace(introductionEntity.NewHireQuestionnaire)
                || string.IsNullOrWhiteSpace(introductionEntity.NewHireUserPrincipalName))
            {
                return false;
            }

            var result = await this.InsertOrReplaceIntroductionAsync(introductionEntity);

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
        /// Get new hire introduction detail from Azure Table Storage.
        /// </summary>
        /// <param name="newJoinerAadObjectId">Azure Active Directory object id of new hire.</param>
        /// <param name="hiringManagerAadObjectId">Azure Active Directory object id of hiring manager.</param>
        /// <returns><see cref="Task"/> A task that represents new hire introduction entity details.</returns>
        public async Task<IntroductionEntity> GetIntroductionDetailAsync(string newJoinerAadObjectId, string hiringManagerAadObjectId)
        {
            if (string.IsNullOrWhiteSpace(newJoinerAadObjectId) || string.IsNullOrWhiteSpace(hiringManagerAadObjectId))
            {
                return null;
            }

            await this.EnsureInitializedAsync();

            var operation = TableOperation.Retrieve<IntroductionEntity>(hiringManagerAadObjectId, newJoinerAadObjectId);
            var data = await this.CloudTable.ExecuteAsync(operation);

            return data.Result as IntroductionEntity;
        }

        /// <summary>
        /// This method is used to get review introductions details for a given hiring manager.
        /// </summary>
        /// <param name="hiringManagerAadObjectId">Azure Active Directory object id of hiring manager .</param>
        /// <returns>List of new hire introduction details.</returns>
        public async Task<IEnumerable<IntroductionEntity>> GetAllIntroductionsAsync(string hiringManagerAadObjectId)
        {
            if (string.IsNullOrWhiteSpace(hiringManagerAadObjectId))
            {
                return null;
            }

            await this.EnsureInitializedAsync();

            var introductionEntity = new List<IntroductionEntity>();
            string partitionKeyCondition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, hiringManagerAadObjectId);
            TableQuery<IntroductionEntity> query = new TableQuery<IntroductionEntity>().Where(partitionKeyCondition);
            TableContinuationToken tableContinuationToken = null;

            do
            {
                var queryResponse = await this.CloudTable.ExecuteQuerySegmentedAsync(query, tableContinuationToken);
                tableContinuationToken = queryResponse.ContinuationToken;
                introductionEntity.AddRange(queryResponse?.Results);
            }
            while (tableContinuationToken != null);

            return introductionEntity as List<IntroductionEntity>;
        }

        /// <summary>
        /// This method is used to get all approved introduction entities where survey notification sent status is pending.
        /// </summary>
        /// <returns>List of introduction entities.</returns>
        public async Task<IEnumerable<IntroductionEntity>> GetAllPendingSurveyIntroductionAsync()
        {
            await this.EnsureInitializedAsync();

            var introductionEntity = new List<IntroductionEntity>();
            string approvalStatuCondition = TableQuery.GenerateFilterConditionForInt("ApprovalStatus", QueryComparisons.Equal, (int)IntroductionStatus.Approved);
            string surveyNotificationStatusCondition = TableQuery.GenerateFilterConditionForInt("SurveyNotificationSentStatus", QueryComparisons.Equal, (int)SurveyNotificationStatus.Pending);
            TableQuery<IntroductionEntity> query = new TableQuery<IntroductionEntity>().Where(approvalStatuCondition).Where(surveyNotificationStatusCondition);
            TableContinuationToken tableContinuationToken = null;

            do
            {
                var queryResponse = await this.CloudTable.ExecuteQuerySegmentedAsync(query, tableContinuationToken);
                tableContinuationToken = queryResponse.ContinuationToken;
                introductionEntity.AddRange(queryResponse?.Results);
            }
            while (tableContinuationToken != null);

            return introductionEntity as List<IntroductionEntity>;
        }

        /// <summary>
        /// This method is used to get filtered review introductions details for a given hiring manager.
        /// </summary>
        /// <param name="hiringManagerAadObjectId">Azure Active Directory object id of hiring manager .</param>
        /// <returns>List of filtered new hire introduction details.</returns>
        public async Task<IEnumerable<IntroductionEntity>> GetFilteredIntroductionsAsync(string hiringManagerAadObjectId)
        {
            if (string.IsNullOrWhiteSpace(hiringManagerAadObjectId))
            {
                return null;
            }

            await this.EnsureInitializedAsync();

            var introductionEntity = new List<IntroductionEntity>();
            string partitionKeyCondition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, hiringManagerAadObjectId);

            string pendingForApprovalCondition = TableQuery.GenerateFilterConditionForInt(nameof(IntroductionEntity.ApprovalStatus), QueryComparisons.Equal, (int)IntroductionStatus.PendingForApproval);
            string tellMeMoreCondition = TableQuery.GenerateFilterConditionForInt(nameof(IntroductionEntity.ApprovalStatus), QueryComparisons.Equal, (int)IntroductionStatus.TellMeMore);
            var combinedStatusFilter = TableQuery.CombineFilters(pendingForApprovalCondition, TableOperators.Or, tellMeMoreCondition);

            var combinedFilter = TableQuery.CombineFilters(partitionKeyCondition, TableOperators.And, combinedStatusFilter);

            TableQuery<IntroductionEntity> query = new TableQuery<IntroductionEntity>().Where(combinedFilter);
            TableContinuationToken tableContinuationToken = null;

            do
            {
                var queryResponse = await this.CloudTable.ExecuteQuerySegmentedAsync(query, tableContinuationToken);
                tableContinuationToken = queryResponse.ContinuationToken;
                introductionEntity.AddRange(queryResponse?.Results);
            }
            while (tableContinuationToken != null);

            return introductionEntity as List<IntroductionEntity>;
        }

        /// <summary>
        /// Stores or update new hire introduction data in Azure Table Storage.
        /// </summary>
        /// <param name="introductionEntity">Holds new hire introduction detail entity data.</param>
        /// <returns>A task that represents introduction entity data is saved or updated.</returns>
        private async Task<TableResult> InsertOrReplaceIntroductionAsync(IntroductionEntity introductionEntity)
        {
            await this.EnsureInitializedAsync();

            TableOperation addOrUpdateOperation = TableOperation.InsertOrReplace(introductionEntity);

            return await this.CloudTable.ExecuteAsync(addOrUpdateOperation);
        }
    }
}
