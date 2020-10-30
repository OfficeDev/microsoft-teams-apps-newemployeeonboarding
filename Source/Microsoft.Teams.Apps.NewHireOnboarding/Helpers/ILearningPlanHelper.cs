// <copyright file="ILearningPlanHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.SharePoint;

    /// <summary>
    /// This interface will contain the helper methods to fetch complete learning plan.
    /// </summary>
    public interface ILearningPlanHelper
    {
        /// <summary>
        /// Get complete learning plans details for new hire.
        /// </summary>
        /// <returns>Complete learning plans details.</returns>
        Task<IEnumerable<LearningPlanListItemField>> GetCompleteLearningPlansAsync();

        /// <summary>
        /// Get learning plan card for selected week and item of the list card.
        /// </summary>
        /// <param name="learningPlan">Learning plan item value.</param>
        /// <returns>Learning plan card as attachment.</returns>
        Task<Attachment> GetLearningPlanCardAsync(string learningPlan);

        /// <summary>
        /// Send learning plan list card for selected week.
        /// </summary>
        /// <param name="turnContext">Complete learning plan data.</param>
        /// <param name="userBotInstalledDate">User bot installed date.</param>
        /// <returns>Learning plan list card as attachment.</returns>
        Task GetWeeklyLearningPlanCardAsync(
            ITurnContext<IMessageActivity> turnContext,
            DateTime? userBotInstalledDate);
    }
}
