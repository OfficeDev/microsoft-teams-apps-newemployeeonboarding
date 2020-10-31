// <copyright file="IntroductionQuestionListItem.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models.SharePoint
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Model for response obtained from Microsoft Graph API for new hire introduction question list item details.
    /// </summary>
    public class IntroductionQuestionListItem
    {
        /// <summary>
        /// Gets or sets odata etag.
        /// </summary>
        [JsonProperty("@odata.etag")]
        public Uri OdataEtag { get; set; }

        /// <summary>
        /// Gets or sets unique Id of list item.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a list of item fields.
        /// </summary>
        [JsonProperty("fields")]
        public IntroductionQuestionListItemField IntroductionQuestionData { get; set; }
    }
}
