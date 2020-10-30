// <copyright file="IntroductionQuestionListDetail.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models.SharePoint
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Model for response obtained from Microsoft Graph API for new hire introduction question list details.
    /// </summary>
    public class IntroductionQuestionListDetail
    {
        /// <summary>
        /// Gets or sets data context.
        /// </summary>
        [JsonProperty("@odata.context")]
        public Uri OdataContext { get; set; }

        /// <summary>
        /// Gets or sets a list of items.
        /// </summary>
#pragma warning disable CA2227 // Collection properties should be read only
        [JsonProperty("value")]
        public List<IntroductionQuestionListItem> ListItems { get; set; }
#pragma warning disable CA2227 // Collection properties should be read only
    }
}
