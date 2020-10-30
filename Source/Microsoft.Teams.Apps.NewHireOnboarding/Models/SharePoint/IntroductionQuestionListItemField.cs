// <copyright file="IntroductionQuestionListItemField.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models.SharePoint
{
    using Newtonsoft.Json;

    /// <summary>
    /// Model for response obtained from Microsoft Graph API for new hire introduction question list item fields details.
    /// </summary>
    public class IntroductionQuestionListItemField
    {
        /// <summary>
        /// Gets or sets question of the list item.
        /// </summary>
        [JsonProperty("Question")]
        public string Question { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether question is soft deleted or not.
        /// </summary>
        [JsonProperty("Active")]
        public bool IsActive { get; set; }
    }
}
