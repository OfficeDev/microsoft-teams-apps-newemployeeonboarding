// <copyright file="ListCardButton.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models.Card
{
    using Newtonsoft.Json;

    /// <summary>
    /// A class that represents a model for list card button.
    /// </summary>
    public class ListCardButton
    {
        /// <summary>
        /// Gets or sets type of button action.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets title of button.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets value of button.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
