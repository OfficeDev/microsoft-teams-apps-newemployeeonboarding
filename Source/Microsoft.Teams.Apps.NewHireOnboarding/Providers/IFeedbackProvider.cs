// <copyright file="IFeedbackProvider.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Providers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;

    /// <summary>
    /// Interface for feedback storage provider.
    /// </summary>
    public interface IFeedbackProvider
    {
        /// <summary>
        /// Store or update feedback in the storage.
        /// </summary>
        /// <param name="feedbackEntity">Represents feedback entity used for storage.</param>
        /// <returns><see cref="Task"/> Returns the status whether feedback entity is stored or not.</returns>
        Task<bool> StoreOrUpdateFeedbackAsync(FeedbackEntity feedbackEntity);

        /// <summary>
        /// Get already saved feedbacks based on the batchId (Mon_Year) from the storage.
        /// </summary>
        /// <param name="batchId">BatchId of the feedback stored in the Azure table Storage.</param>
        /// <returns><see cref="Task"/>Returns list of feedback entities.</returns>
        Task<IEnumerable<FeedbackEntity>> GetFeedbackAsync(string batchId);
    }
}
