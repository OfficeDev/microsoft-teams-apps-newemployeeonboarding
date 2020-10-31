// <copyright file="LearningPlanColumnDetail.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Models.SharePoint
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Model for response obtained from Microsoft Graph API for complete learning plan list columns.
    /// </summary>
    public class LearningPlanColumnDetail
    {
        /// <summary>
        /// Gets or sets data context.
        /// </summary>
        [JsonProperty("@odata.context")]
        public Uri OdataContext { get; set; }

        /// <summary>
        /// Gets or sets a list of column mappings.
        /// </summary>
#pragma warning disable CA2227 // Collection properties should be read only
        [JsonProperty("value")]
        public List<LearningPlanColumnMap> ColumnMappings { get; set; }
#pragma warning disable CA2227 // Collection properties should be read only
    }
}
