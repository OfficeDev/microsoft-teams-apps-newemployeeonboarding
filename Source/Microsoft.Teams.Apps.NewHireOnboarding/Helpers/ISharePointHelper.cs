// <copyright file="ISharePointHelper.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Helpers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.SharePoint;

    /// <summary>
    /// This interface will contain the helper methods for SharePoint.
    /// </summary>
    public interface ISharePointHelper
    {
        /// <summary>
        /// Get list of complete learning plan details.
        /// </summary>
        /// <param name="token">Azure Active Directory (AAD) token to access Microsoft Graph API.</param>
        /// <returns>A task that returns list of learning plan details.</returns>
        Task<List<LearningPlanListItemField>> GetCompleteLearningPlanDataAsync(string token);

        /// <summary>
        /// Get new hire introduction questions from SharePoint site.
        /// </summary>
        /// <param name="token">Azure Active Directory (AAD) token to access Microsoft Graph API.</param>
        /// <returns>A task that returns list of introduction questions.</returns>
        Task<IEnumerable<IntroductionDetail>> GetIntroductionQuestionsAsync(string token);
    }
}
