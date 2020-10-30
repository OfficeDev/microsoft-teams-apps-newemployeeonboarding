// <copyright file="LearningPlanHelperData.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.NewHireOnboarding.Tests.TestData
{
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.Graph;
    using Microsoft.Teams.Apps.NewHireOnboarding.Models.SharePoint;
    using System.Collections.Generic;

    /// <summary>
    /// Learning plan helper test data.
    /// </summary>
    public static class LearningPlanHelperData
    {
        public static readonly List<LearningPlanListItemField> learningPlanListDetail = new List<LearningPlanListItemField>()
        {
            new LearningPlanListItemField()
            {
                CompleteBy = "Week 1",
                Topic = "Technology",
                TaskName = "ReactJS",
                Link = new LearningPlanResource()
                {
                    Description = "",
                    Url = ""
                }
            },
            new LearningPlanListItemField()
            {
                CompleteBy = "Week 2",
                Topic = "Management",
                TaskName = "Team management",
                Link = new LearningPlanResource()
                {
                    Description = "",
                    Url = ""
                }
            }
        };

        /// <summary>
        /// Tenant id of application.
        /// </summary>
        public static readonly string TenantId = "<<TenantId>>";

        /// <summary>
        /// Application client id.
        /// </summary>
        public static readonly string ClientId = "<<ClientId>>";

        /// <summary>
        /// Application secret id.
        /// </summary>
        public static readonly string ClientSecret = "<<ClientSecret>>";

        public static readonly GraphTokenResponse graphTokenResponse = new GraphTokenResponse()
        {
            AccessToken = "<<Graph access token>>",
        };
    }
}
