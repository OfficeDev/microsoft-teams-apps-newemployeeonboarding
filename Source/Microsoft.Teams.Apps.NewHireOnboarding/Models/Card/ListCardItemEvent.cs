// <copyright file="ListCardItemEvent.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models.Card
{
    using Newtonsoft.Json;

    /// <summary>
    /// A class that represent the list card item event model.
    /// </summary>
    public class ListCardItemEvent
    {
        /// <summary>
        /// Gets or sets type for item tap event on list card.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets value for item tap event on list card.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
