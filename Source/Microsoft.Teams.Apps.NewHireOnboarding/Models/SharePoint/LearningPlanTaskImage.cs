// <copyright file="LearningPlanTaskImage.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models.SharePoint
{
    using Newtonsoft.Json;

    /// <summary>
    /// Model for response obtained from Microsoft Graph API for task image.
    /// </summary>
    public class LearningPlanTaskImage
    {
        /// <summary>
        /// Gets or sets Url of task image.
        /// </summary>
        [JsonProperty("Url")]
        public string Url { get; set; }
    }
}
