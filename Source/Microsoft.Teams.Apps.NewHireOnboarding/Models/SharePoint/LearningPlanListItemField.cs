// <copyright file="LearningPlanListItemField.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models.SharePoint
{
    /// <summary>
    /// Model for response obtained from Microsoft Graph API for complete learning plan list item fields details.
    /// </summary>
    public class LearningPlanListItemField
    {
        /// <summary>
        /// Gets or sets title of the list item.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Gets or sets task name of list item.
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// Gets or sets complete by of list item.
        /// </summary>
        public string CompleteBy { get; set; }

        /// <summary>
        /// Gets or sets display name of list.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets task image of learning plan.
        /// </summary>
        public LearningPlanTaskImage TaskImage { get; set; }

        /// <summary>
        /// Gets or sets resource link.
        /// </summary>
        public LearningPlanResource Link { get; set; }
    }
}
