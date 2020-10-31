// <copyright file="LearningPlanResource.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models.SharePoint
{
    using Newtonsoft.Json;

    /// <summary>
    /// Model for response obtained from Microsoft Graph API for resource link.
    /// </summary>
    public class LearningPlanResource
    {
        /// <summary>
        /// Gets or sets description of resource link.
        /// </summary>
        [JsonProperty("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Url of resource.
        /// </summary>
        [JsonProperty("Url")]
        public string Url { get; set; }
    }
}
