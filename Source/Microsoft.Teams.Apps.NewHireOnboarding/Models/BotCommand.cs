// <copyright file="BotCommand.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models
{
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.EntityModels;
    using Newtonsoft.Json;

    /// <summary>
    /// A class that represents properties to be parsed from activity value.
    /// </summary>
    public class BotCommand
    {
        /// <summary>
        /// Gets or sets bot command text.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets introduction details.
        /// </summary>
        [JsonProperty("introductionEntity")]
        public IntroductionEntity IntroductionEntity { get; set; }
    }
}
