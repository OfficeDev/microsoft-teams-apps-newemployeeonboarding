// <copyright file="SharePointSettings.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models.Configuration
{
    /// <summary>
    /// A class which helps to provide SharePoint settings for application.
    /// </summary>
    public class SharePointSettings
    {
        /// <summary>
        /// Gets or sets SharePoint site name.
        /// </summary>
        public string SiteName { get; set; }

        /// <summary>
        /// Gets or sets SharePoint new hire check list name for learning data.
        /// </summary>
        public string NewHireCheckListName { get; set; }

        /// <summary>
        /// Gets or sets SharePoint site tenant name.
        /// </summary>
        public string SiteTenantName { get; set; }

        /// <summary>
        /// Gets or sets share feedback form URL.
        /// </summary>
        public string ShareFeedbackFormUrl { get; set; }

        /// <summary>
        /// Gets or sets complete learning plan SharePoint URL.
        /// </summary>
        public string CompleteLearningPlanUrl { get; set; }

        /// <summary>
        /// Gets or sets SharePoint site list name for new hire questions.
        /// </summary>
        public string NewHireQuestionListName { get; set; }

        /// <summary>
        /// Gets or sets total number of learning plan notification weeks.
        /// </summary>
        public int NewHireLearningPlansInWeeks { get; set; }
    }
}
