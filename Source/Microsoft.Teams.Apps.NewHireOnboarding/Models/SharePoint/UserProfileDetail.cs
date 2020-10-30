// <copyright file="UserProfileDetail.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models.SharePoint
{
    using Newtonsoft.Json;

    /// <summary>
    /// User profile details model class for Microsoft Graph API.
    /// </summary>
    public class UserProfileDetail
    {
        /// <summary>
        /// Gets or sets odataContext.
        /// </summary>
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }

        /// <summary>
        /// Gets or sets user unique id.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets job title.
        /// </summary>
        [JsonProperty("jobTitle")]
        public string JobTitle { get; set; }
    }
}
