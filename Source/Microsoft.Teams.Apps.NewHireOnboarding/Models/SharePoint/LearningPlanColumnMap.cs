// <copyright file="LearningPlanColumnMap.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models.SharePoint
{
    using Newtonsoft.Json;

    /// <summary>
    /// Model for response obtained from Microsoft Graph API for SharePoing list column mappings.
    /// </summary>
    public class LearningPlanColumnMap
    {
        /// <summary>
        /// Gets or sets display name of column.
        /// </summary>
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets actual internally generated name of column.
        /// </summary>
        [JsonProperty("name")]
        public string ActualName { get; set; }
    }
}
