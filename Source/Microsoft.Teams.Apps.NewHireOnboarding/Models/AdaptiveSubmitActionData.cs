// <copyright file="AdaptiveSubmitActionData.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models
{
    using Microsoft.Bot.Schema;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines teams-specific behavior for an adaptive card submit action.
    /// </summary>
    public class AdaptiveSubmitActionData
    {
        /// <summary>
        /// Gets or sets the teams specific action.
        /// </summary>
        [JsonProperty("msteams")]
        public CardAction Msteams { get; set; }

        /// <summary>
        /// Gets or sets introduction details.
        /// </summary>
        [JsonProperty("introductionEntity")]
        public IntroductionEntity IntroductionEntity { get; set; }

        /// <summary>
        /// Gets or sets comments from hiring manager.
        /// </summary>
        [JsonProperty("comments")]
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets command text to show submit or cancel event on Task Module.
        /// </summary>
        [JsonProperty("command")]
        public string Command { get; set; }
    }
}
