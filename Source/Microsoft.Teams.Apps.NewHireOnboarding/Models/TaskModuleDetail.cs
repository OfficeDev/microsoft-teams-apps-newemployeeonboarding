// <copyright file="TaskModuleDetail.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models
{
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;
    using Newtonsoft.Json;

    /// <summary>
    /// Class which holds task module details.
    /// </summary>
    public class TaskModuleDetail
    {
        /// <summary>
        /// Gets or sets command to show submit or cancel event on Task Module.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets introduction details.
        /// </summary>
        [JsonProperty("introductionEntity")]
        public IntroductionEntity IntroductionEntity { get; set; }

        /// <summary>
        /// Gets or sets team id.
        /// </summary>
        [JsonProperty("teamId")]
        public string TeamId { get; set; }
    }
}
