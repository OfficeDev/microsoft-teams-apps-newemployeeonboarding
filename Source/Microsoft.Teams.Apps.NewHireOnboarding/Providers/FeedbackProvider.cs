// <copyright file="FeedbackProvider.cs" company="Microsoft">
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
    /// Implements the methods that are defined in <see cref="IFeedbackProvider"/>.
    /// Implements storage provider which helps to store feedback in Azure Table Storage.
    /// </summary>
    public class FeedbackProvider : BaseStorageProvider, IFeedbackProvider
    {
        private const string FeedbackTable = "Feedback";

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedbackProvider"/> class.
        /// </summary>
        /// <param name="options">A set of key/value application configuration properties for Azure Table Storage.</param>
        /// <param name="logger">Logger implementation to send logs to the logger service.</param>
        public FeedbackProvider(
            IOptions<StorageSettings> options,
            ILogger<FeedbackProvider> logger)
            : base(options?.Value.ConnectionString, FeedbackTable, logger)
        {
        }

        /// <summary>
        /// Store or update feedback in the storage.
        /// </summary>
        /// <param name="feedbackEntity">Represents feedback entity used for storage.</param>
        /// <returns><see cref="Task"/> Returns the status whether feedback entity is stored or not.</returns>
        public async Task<bool> StoreOrUpdateFeedbackAsync(FeedbackEntity feedbackEntity)
        {
            feedbackEntity = feedbackEntity ?? throw new ArgumentNullException(nameof(feedbackEntity));

            var result = await this.StoreOrUpdateEntityAsync(feedbackEntity);

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
        /// Get already saved feedbacks based on the batchId (Mon_Year) from the storage.
        /// </summary>
        /// <param name="batchId">BatchId of the feedback stored in the Azure table Storage.</param>
        /// <returns><see cref="Task"/>Returns list of feedback entities.</returns>
        public async Task<IEnumerable<FeedbackEntity>> GetFeedbackAsync(string batchId)
        {
            if (string.IsNullOrWhiteSpace(batchId))
            {
                return null;
            }

            await this.EnsureInitializedAsync();

            var feedbackEntity = new List<FeedbackEntity>();
            string partitionKeyCondition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, batchId);
            TableQuery<FeedbackEntity> query = new TableQuery<FeedbackEntity>().Where(partitionKeyCondition);
            TableContinuationToken tableContinuationToken = null;

            do
            {
                var queryResponse = await this.CloudTable.ExecuteQuerySegmentedAsync(query, tableContinuationToken);
                tableContinuationToken = queryResponse.ContinuationToken;
                feedbackEntity.AddRange(queryResponse?.Results);
            }
            while (tableContinuationToken != null);

            return feedbackEntity;
        }

        /// <summary>
        /// Stores or update feedback data in Azure Table Storage.
        /// </summary>
        /// <param name="feedbackEntity">Holds feedback entity data.</param>
        /// <returns>A task that represents feedback entity data is saved or updated.</returns>
        private async Task<TableResult> StoreOrUpdateEntityAsync(FeedbackEntity feedbackEntity)
        {
            await this.EnsureInitializedAsync();

            TableOperation addOrUpdateOperation = TableOperation.InsertOrReplace(feedbackEntity);

            return await this.CloudTable.ExecuteAsync(addOrUpdateOperation);
        }
    }
}
